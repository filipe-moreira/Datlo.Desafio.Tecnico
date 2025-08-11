using CsvHelper.Configuration;
using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Application.Services.FileProcessors.TrafegoPessoas.Csv;
using Datlo.TesteTecnico.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Globalization;
using System.Text;
using Xunit;

namespace Datlo.TesteTecnico.Tests.Services.FileProcessors.TrafegoPessoas
{
    public class TrafegoPessoaCsvFileProcessorTests
    {
        private readonly Mock<ILogger<TrafegoPessoaCsvFileProcessor>> _loggerMock;
        private readonly Mock<ITrafegoPessoaCsvMapper> _mapperMock;
        private readonly TrafegoPessoaCsvFileProcessor _processor;

        public TrafegoPessoaCsvFileProcessorTests()
        {
            _loggerMock = new Mock<ILogger<TrafegoPessoaCsvFileProcessor>>();
            _mapperMock = new Mock<ITrafegoPessoaCsvMapper>();
            _processor = new TrafegoPessoaCsvFileProcessor(_loggerMock.Object);

            // Configuração padrão do mapper mock
            _mapperMock.Setup(m => m.TipoArquivo).Returns(TipoArquivo.Csv);
            _mapperMock.Setup(m => m.ClienteId).Returns("TEST_CLIENT");
            _mapperMock.Setup(m => m.ConfiguracaoCsv).Returns(new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                DetectColumnCountChanges = false,
                MissingFieldFound = null
            });
        }

        [Fact]
        public async Task ProcessarAsync_ComDadosValidos_DeveProcessarTodosRegistros()
        {
            // Arrange
            var csvContent = "data,qtd,lat,lon\n2024-01-01,100,-23.5,-46.6\n2024-01-02,200,-23.6,-46.7";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var dto1 = new TrafegoPessoaDto(new DateTime(2024, 1, 1), 100, -23.5, -46.6);
            var dto2 = new TrafegoPessoaDto(new DateTime(2024, 1, 2), 200, -23.6, -46.7);

            _mapperMock.SetupSequence(m => m.Map(It.IsAny<CsvHelper.CsvReader>()))
                .Returns(dto1)
                .Returns(dto2);

            _mapperMock.Setup(m => m.Validate(It.IsAny<TrafegoPessoaDto>())); // Não lança exceção

            // Act
            var resultados = new List<Domain.Entidades.TrafegoPessoa>();
            await foreach (var entidade in _processor.ProcessarAsync(stream, _mapperMock.Object, "TEST_CLIENT", "test.csv", CancellationToken.None))
            {
                resultados.Add(entidade);
            }

            // Assert
            resultados.Should().HaveCount(2);
            resultados[0].QtdEstimadaPessoas.Should().Be(100);
            resultados[1].QtdEstimadaPessoas.Should().Be(200);

            // Verifica se o log de conclusão foi chamado
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Processamento do arquivo") && v.ToString().Contains("concluído")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessarAsync_ComDadosInvalidos_DeveContinuarProcessamentoELogarAlertas()
        {
            // Arrange
            var csvContent = "data,qtd,lat,lon\n2024-01-01,100,-23.5,-46.6\n2024-01-02,-200,-23.6,-46.7\n2024-01-03,300,-23.7,-46.8";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var dto1 = new TrafegoPessoaDto(new DateTime(2024, 1, 1), 100, -23.5, -46.6);
            var dto2 = new TrafegoPessoaDto(new DateTime(2024, 1, 2), -200, -23.6, -46.7); // Inválido
            var dto3 = new TrafegoPessoaDto(new DateTime(2024, 1, 3), 300, -23.7, -46.8);

            _mapperMock.SetupSequence(m => m.Map(It.IsAny<CsvHelper.CsvReader>()))
                .Returns(dto1)
                .Returns(dto2)
                .Returns(dto3);

            _mapperMock.Setup(m => m.Validate(dto1)); // Válido
            _mapperMock.Setup(m => m.Validate(dto2)).Throws(new ArgumentException("QtdEstimadaPessoas não pode ser negativa.")); // Inválido
            _mapperMock.Setup(m => m.Validate(dto3)); // Válido

            // Act
            var resultados = new List<Domain.Entidades.TrafegoPessoa>();
            await foreach (var entidade in _processor.ProcessarAsync(stream, _mapperMock.Object, "TEST_CLIENT", "test.csv", CancellationToken.None))
            {
                resultados.Add(entidade);
            }

            // Assert
            resultados.Should().HaveCount(2); // Apenas os registros válidos
            resultados[0].QtdEstimadaPessoas.Should().Be(100);
            resultados[1].QtdEstimadaPessoas.Should().Be(300);

            // Verifica se o log de warning foi chamado para o registro inválido
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Registro inválido encontrado na linha 3")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            // Verifica se o log final mostra as estatísticas corretas
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Registros processados: 2") && v.ToString().Contains("Registros inválidos: 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Constructor_ComLoggerNulo_DeveLancarArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new TrafegoPessoaCsvFileProcessor(null));
            exception.ParamName.Should().Be("logger");
        }

        [Fact]
        public async Task ProcessarAsync_ComMapperInvalido_DeveLancarInvalidOperationException()
        {
            // Arrange
            var mapperInvalido = new Mock<ITrafegoPessoaMapper>();
            mapperInvalido.Setup(m => m.TipoArquivo).Returns(TipoArquivo.Csv);
            mapperInvalido.Setup(m => m.ClienteId).Returns("TEST_CLIENT");

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("data,qtd,lat,lon\n"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await foreach (var _ in _processor.ProcessarAsync(stream, mapperInvalido.Object, "TEST_CLIENT", "test.csv", CancellationToken.None))
                {
                    // Não deve chegar aqui
                }
            });

            exception.Message.Should().Contain("não implementa ITrafegoPessoaCsvMapper");
        }
    }
}
