using CsvHelper.Configuration;
using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal.Csv;
using Datlo.TesteTecnico.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Globalization;
using System.Text;
using Xunit;

namespace Datlo.TesteTecnico.Tests.Services.FileProcessors.DemografiaLocal
{
    public class DemografiaLocalCsvFileProcessorTests
    {
        private readonly Mock<ILogger<DemografiaLocalCsvFileProcessor>> _loggerMock;
        private readonly Mock<IDemografiaLocalCsvMapper> _mapperMock;
        private readonly DemografiaLocalCsvFileProcessor _processor;

        public DemografiaLocalCsvFileProcessorTests()
        {
            _loggerMock = new Mock<ILogger<DemografiaLocalCsvFileProcessor>>();
            _mapperMock = new Mock<IDemografiaLocalCsvMapper>();
            _processor = new DemografiaLocalCsvFileProcessor(_loggerMock.Object);

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
            var csvContent = "data,lat,lon,populacao,renda,faixa\n2024-01-01,-23.5,-46.6,10000,3500.50,25-35\n2024-01-02,-23.6,-46.7,15000,4000.75,35-45";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var dto1 = new DemografiaLocalDto(new DateTime(2024, 1, 1), -23.5, -46.6, 10000, 3500.50, "25-35");
            var dto2 = new DemografiaLocalDto(new DateTime(2024, 1, 2), -23.6, -46.7, 15000, 4000.75, "35-45");

            _mapperMock.SetupSequence(m => m.Map(It.IsAny<CsvHelper.CsvReader>()))
                .Returns(dto1)
                .Returns(dto2);

            _mapperMock.Setup(m => m.Validate(It.IsAny<DemografiaLocalDto>())); // Não lança exceção

            // Act
            var resultados = new List<Datlo.TesteTecnico.Domain.Entidades.DemografiaLocal>();
            await foreach (var entidade in _processor.ProcessarAsync(stream, _mapperMock.Object, "TEST_CLIENT", "test.csv", CancellationToken.None))
            {
                resultados.Add(entidade);
            }

            // Assert
            resultados.Should().HaveCount(2);
            resultados[0].Populacao.Should().Be(10000);
            resultados[1].Populacao.Should().Be(15000);

            // Verifica se o log de conclusão foi chamado
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processamento do arquivo") && v.ToString()!.Contains("concluído")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessarAsync_ComDadosInvalidos_DeveContinuarProcessamentoELogarAlertas()
        {
            // Arrange
            var csvContent = "data,lat,lon,populacao,renda,faixa\n2024-01-01,-23.5,-46.6,10000,3500.50,25-35\n2024-01-02,-23.6,-46.7,-15000,4000.75,35-45\n2024-01-03,-23.7,-46.8,20000,5000.25,45-55";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var dto1 = new DemografiaLocalDto(new DateTime(2024, 1, 1), -23.5, -46.6, 10000, 3500.50, "25-35");
            var dto2 = new DemografiaLocalDto(new DateTime(2024, 1, 2), -23.6, -46.7, -15000, 4000.75, "35-45"); // Inválido
            var dto3 = new DemografiaLocalDto(new DateTime(2024, 1, 3), -23.7, -46.8, 20000, 5000.25, "45-55");

            _mapperMock.SetupSequence(m => m.Map(It.IsAny<CsvHelper.CsvReader>()))
                .Returns(dto1)
                .Returns(dto2)
                .Returns(dto3);

            _mapperMock.Setup(m => m.Validate(dto1)); // Válido
            _mapperMock.Setup(m => m.Validate(dto2)).Throws(new ArgumentException("População não pode ser negativa.")); // Inválido
            _mapperMock.Setup(m => m.Validate(dto3)); // Válido

            // Act
            var resultados = new List<Datlo.TesteTecnico.Domain.Entidades.DemografiaLocal>();
            await foreach (var entidade in _processor.ProcessarAsync(stream, _mapperMock.Object, "TEST_CLIENT", "test.csv", CancellationToken.None))
            {
                resultados.Add(entidade);
            }

            // Assert
            resultados.Should().HaveCount(2); // Apenas os registros válidos
            resultados[0].Populacao.Should().Be(10000);
            resultados[1].Populacao.Should().Be(20000);

            // Verifica se o log de warning foi chamado para o registro inválido
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Registro inválido encontrado na linha 3")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            // Verifica se o log final mostra as estatísticas corretas
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Registros processados: 2") && v.ToString()!.Contains("Registros inválidos: 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void Constructor_ComLoggerNulo_DeveLancarArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new DemografiaLocalCsvFileProcessor(null!));
            exception.ParamName.Should().Be("logger");
        }

        [Fact]
        public async Task ProcessarAsync_ComMapperInvalido_DeveLancarInvalidOperationException()
        {
            // Arrange
            var mapperInvalido = new Mock<IDemografiaLocalMapper>();
            mapperInvalido.Setup(m => m.TipoArquivo).Returns(TipoArquivo.Csv);
            mapperInvalido.Setup(m => m.ClienteId).Returns("TEST_CLIENT");

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("data,lat,lon,populacao,renda,faixa\n"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await foreach (var _ in _processor.ProcessarAsync(stream, mapperInvalido.Object, "TEST_CLIENT", "test.csv", CancellationToken.None))
                {
                    // Não deve chegar aqui
                }
            });

            exception.Message.Should().Contain("não implementa IDemografiaLocalCsvMapper");
        }
    }
}
