using Datlo.TesteTecnico.Api.Models;
using Datlo.TesteTecnico.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Datlo.TesteTecnico.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // [Authorize]
    // EM UM AMBIENTE REAL, O ACESSO A ESTE CONTROLLER DEVE SER PROTEGIDO POR AUTENTICAÇÃO (JWT, POR EXEMPLO)
    public class S3Controller : ControllerBase
    {
        private readonly IS3Services _s3Services;
        private readonly ILogger<S3Controller> _logger;

        public S3Controller(IS3Services s3Services, ILogger<S3Controller> logger)
        {
            _s3Services = s3Services ?? throw new ArgumentNullException(nameof(s3Services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("gerar-url-assinada")]
        public async Task<IActionResult> GerarUrlAssinada([FromBody] GerarUrlAssinadaRequest request)
        {
            /// ao salvar o arquivo, 
            /// o clienteId (identificador do cliente que está enviando o arquivo) 
            /// seria obtido do CONTEXTO DE AUTENTICAÇÃO e passado para o serviço S3

            var clienteId = "ID_CLIENTE_C"; /// User?.FindFirst("clienteId")?.Value;

            _logger.LogInformation(
                "Iniciando geração de URL pré-assinada para ClienteId={ClienteId}, TipoArquivo={TipoArquivo}, ModeloArquivo={ModeloArquivo}",
                clienteId,
                request.TipoArquivo,
                request.ModeloArquivo
            );

            try
            {
                var urlAssinada = await _s3Services.GerarUrlAssinada(
                    clienteId,
                    request.TipoArquivo,
                    request.ModeloArquivo
                );

                _logger.LogInformation(
                    "URL pré-assinada gerada com sucesso para ClienteId={ClienteId}, TipoArquivo={TipoArquivo}, ModeloArquivo={ModeloArquivo}",
                    clienteId,
                    request.TipoArquivo,
                    request.ModeloArquivo
                );

                return Ok(new { urlAssinada });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao gerar URL pré-assinada para ClienteId={ClienteId}, TipoArquivo={TipoArquivo}, ModeloArquivo={ModeloArquivo}",
                    clienteId,
                    request.TipoArquivo,
                    request.ModeloArquivo
                );
                return StatusCode(500, "Erro interno do servidor");
            }
        }

    }
}
