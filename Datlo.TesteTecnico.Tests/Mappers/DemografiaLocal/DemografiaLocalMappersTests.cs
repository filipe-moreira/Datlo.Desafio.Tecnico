using CsvHelper;
using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Csv.ClienteC;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Csv.ClienteD;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Json.ClienteA;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Json.ClienteB;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Xml.ClienteE;
using Datlo.TesteTecnico.Domain.Models;
using FluentAssertions;
using System.Text.Json;
using System.Xml;

namespace Datlo.TesteTecnico.Tests.Mappers.DemografiaLocal
{
    public class DemografiaLocalMappersTests
    {
        [Fact]
        public void DemografiaLocalMapperClienteA_DeveMapearJsonCorretamente()
        {
            // Arrange
            var mapper = new DemografiaLocalMapperClienteA();
            var jsonString = """
                {
                    "data_registro": "2024-01-15T10:30:00",
                    "latitude": -23.5505,
                    "longitude": -46.6333,
                    "populacao": 15000,
                    "renda_media": 3500.50,
                    "faixa_etaria_predominante": "35"
                }
                """;
            var jsonElement = JsonDocument.Parse(jsonString).RootElement;

            // Act
            var resultado = mapper.Map(jsonElement);

            // Assert
            resultado.DataRegistro.Should().Be(new DateTime(2024, 1, 15, 10, 30, 0));
            resultado.Lat.Should().BeApproximately(-23.5505, 0.0001);
            resultado.Lon.Should().BeApproximately(-46.6333, 0.0001);
            resultado.Populacao.Should().Be(15000);
            resultado.RendaMedia.Should().BeApproximately(3500.50, 0.01);            
        }

        [Fact]
        public void DemografiaLocalMapperClienteB_DeveMapearGeoJsonCorretamente()
        {
            // Arrange
            var mapper = new DemografiaLocalMapperClienteB();
            var geoJsonString = """
                {
                    "type": "Feature",
                    "properties": {
                        "data_coleta": "2024-01-15T00:00:00",
                        "total_habitantes": 25000,
                        "renda_familiar_media": 4200.75,
                        "idade_media": "42"
                    },
                    "geometry": {
                        "type": "Point",
                        "coordinates": [-46.6333, -23.5505]
                    }
                }
                """;
            var jsonElement = JsonDocument.Parse(geoJsonString).RootElement;

            // Act
            var resultado = mapper.Map(jsonElement);

            // Assert
            resultado.DataRegistro.Should().Be(new DateTime(2024, 1, 15));
            resultado.Lat.Should().BeApproximately(-23.5505, 0.0001);
            resultado.Lon.Should().BeApproximately(-46.6333, 0.0001);
            resultado.Populacao.Should().Be(25000);
            resultado.RendaMedia.Should().BeApproximately(4200.75, 0.01);            
        }

        [Fact]
        public void DemografiaLocalMapperClienteC_DeveMapearCsvCorretamente()
        {
            // Arrange
            var mapper = new DemografiaLocalMapperClienteC();
            var csvData = "2024-01-15;-23.5505;-46.6333;15000;3500.50;35";
            
            using var reader = new StringReader(csvData);
            using var csv = new CsvReader(reader, mapper.ConfiguracaoCsv);
            csv.Read();

            // Act
            var resultado = mapper.Map(csv);

            // Assert
            resultado.DataRegistro.Should().Be(new DateTime(2024, 1, 15));
            resultado.Lat.Should().BeApproximately(-23.5505, 0.0001);
            resultado.Lon.Should().BeApproximately(-46.6333, 0.0001);
            resultado.Populacao.Should().Be(15000);
            resultado.RendaMedia.Should().BeApproximately(3500.50, 0.01);            
        }

        [Fact]
        public void DemografiaLocalMapperClienteD_DeveMapearCsvComOrdemDiferenteCorretamente()
        {
            // Arrange
            var mapper = new DemografiaLocalMapperClienteD();
            var csvData = "15000;3500.50;35;2024-01-15 10:30:00;-23.5505;-46.6333";
            
            using var reader = new StringReader(csvData);
            using var csv = new CsvReader(reader, mapper.ConfiguracaoCsv);
            csv.Read();

            // Act
            var resultado = mapper.Map(csv);

            // Assert
            resultado.DataRegistro.Should().Be(new DateTime(2024, 1, 15, 10, 30, 0));
            resultado.Lat.Should().BeApproximately(-23.5505, 0.0001);
            resultado.Lon.Should().BeApproximately(-46.6333, 0.0001);
            resultado.Populacao.Should().Be(15000);
            resultado.RendaMedia.Should().BeApproximately(3500.50, 0.01);            
        }

        [Fact]
        public void DemografiaLocalMapperClienteE_DeveMapearXmlCorretamente()
        {
            // Arrange
            var mapper = new DemografiaLocalMapperClienteE();
            var xmlString = """
                <demografia data="2024-01-15T10:30:00" lat="-23.5505" lon="-46.6333" 
                            populacao="15000" renda="3500.50" idade="35" />
                """;
            
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            var element = xmlDoc.DocumentElement!;

            // Act
            var resultado = mapper.Map(element);

            // Assert
            resultado.DataRegistro.Should().Be(new DateTime(2024, 1, 15, 10, 30, 0));
            resultado.Lat.Should().BeApproximately(-23.5505, 0.0001);
            resultado.Lon.Should().BeApproximately(-46.6333, 0.0001);
            resultado.Populacao.Should().Be(15000);
            resultado.RendaMedia.Should().BeApproximately(3500.50, 0.01);            
        }

        [Fact]
        public void DemografiaLocalMapperClienteA_DeveValidarDadosCorretamente()
        {
            // Arrange
            var mapper = new DemografiaLocalMapperClienteA();
            var dtoValido = new DemografiaLocalDto(
                DateTime.Now, -23.5505, -46.6333, 15000, 3500.50, "");
            var dtoInvalido = new DemografiaLocalDto(
                DateTime.MinValue, -23.5505, -46.6333, -1000, -100, "");

            // Act & Assert
            var act = () => mapper.Validate(dtoInvalido);
            act.Should().Throw<ArgumentException>();

            // Não deve lançar exceção para dados válidos
            var actValido = () => mapper.Validate(dtoValido);
            actValido.Should().NotThrow();
        }

        [Fact]
        public void DemografiaLocalMapperClienteD_DeveAplicarValidacaoRigorosa()
        {
            // Arrange
            var mapper = new DemografiaLocalMapperClienteD();
            var dtoPopulacaoBaixa = new DemografiaLocalDto(
                DateTime.Now, -23.5505, -46.6333, 500, 3500.50, "15-25"); // População < 1000
            var dtoRendaBaixa = new DemografiaLocalDto(
                DateTime.Now, -23.5505, -46.6333, 15000, 500, "20-35"); // Renda < 1000

            // Act & Assert
            var actPopulacao = () => mapper.Validate(dtoPopulacaoBaixa);
            actPopulacao.Should().Throw<ArgumentException>();

            var actRenda = () => mapper.Validate(dtoRendaBaixa);
            actRenda.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void TodosOsMappers_DevemTerPropriedadesCorretas()
        {
            // Arrange & Act & Assert
            var mapperA = new DemografiaLocalMapperClienteA();
            mapperA.ClienteId.Should().Be("ID_CLIENTE_A");
            mapperA.TipoArquivo.Should().Be(TipoArquivo.Json);

            var mapperB = new DemografiaLocalMapperClienteB();
            mapperB.ClienteId.Should().Be("ID_CLIENTE_B");
            mapperB.TipoArquivo.Should().Be(TipoArquivo.Json);

            var mapperC = new DemografiaLocalMapperClienteC();
            mapperC.ClienteId.Should().Be("ID_CLIENTE_C");
            mapperC.TipoArquivo.Should().Be(TipoArquivo.Csv);

            var mapperD = new DemografiaLocalMapperClienteD();
            mapperD.ClienteId.Should().Be("ID_CLIENTE_D");
            mapperD.TipoArquivo.Should().Be(TipoArquivo.Csv);

            var mapperE = new DemografiaLocalMapperClienteE();
            mapperE.ClienteId.Should().Be("ID_CLIENTE_E");
            mapperE.TipoArquivo.Should().Be(TipoArquivo.Xml);
        }
    }
}
