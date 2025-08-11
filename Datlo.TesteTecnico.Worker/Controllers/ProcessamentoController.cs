using Datlo.TesteTecnico.Domain.Models;
using Datlo.TesteTecnico.Worker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datlo.TesteTecnico.Worker.Controllers
{
    /// <summary>
    /// Controller para testar o enfileiramento de jobs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessamentoController : ControllerBase
    {
        private readonly IJobEnqueueService _jobEnqueueService;
        private readonly ILogger<ProcessamentoController> _logger;

        public ProcessamentoController(
            IJobEnqueueService jobEnqueueService,
            ILogger<ProcessamentoController> logger)
        {
            ArgumentNullException.ThrowIfNull(jobEnqueueService);
            ArgumentNullException.ThrowIfNull(logger);

            _jobEnqueueService = jobEnqueueService;
            _logger = logger;
        }

        /// <summary>
        /// Enfileira um job para processar tráfego de pessoas
        /// </summary>
        [HttpPost("enfileirar-jobs-teste")]
        public IActionResult EnfileirarTrafegoPessoa()
        {
            try
            {
                EnfileirarJobsExemplo();

                return Ok("Job's enfileirados com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enfileirar jobs");
                return StatusCode(500, new { Message = "Erro interno do servidor" });
            }
        }

        // Exemplo de como enfileirar os jobs
        // em um cenário real, a obtenção dos arquivos viria de uma fonte externa
        // como uma API, banco de dados, ou listagem do S3 (que no projeto atual é o que esta sendo feito)
        private void EnfileirarJobsExemplo()
        {
            try
            {
                // Exemplo de arquivos para processar (em um ambiente real, viria de uma fonte externa)
                var arquivos = new List<ArquivosS3>
                {        
                    new ArquivosS3("ID_CLIENTE_C", "datlo.teste.tecnico", "cliente_c_trafego_pessoas_e0996b32-46d6-45d9-b83e-7ef6a7e44f5e.csv", TipoArquivo.Csv, ModeloArquivo.TrafegoPessoas),
                    //new ArquivosS3("ID_CLIENTE_D", "datlo.teste.tecnico", "cliente_d_trafego_pessoas_6a604603-fe8e-494f-b693-a77961d1e5e0.csv", TipoArquivo.Csv, ModeloArquivo.TrafegoPessoas),
                    //new ArquivosS3("ID_CLIENTE_C", "datlo.teste.tecnico", "cliente_c_demografia_local_a3195f57-662c-4ca9-aa36-8f14412bf367.csv", TipoArquivo.Csv, ModeloArquivo.DemografiaLocal),
                    //new ArquivosS3("ID_CLIENTE_D", "datlo.teste.tecnico", "cliente_d_demografia_local_635ac585-1077-4729-ae41-80fbd729cb4f.csv", TipoArquivo.Csv, ModeloArquivo.DemografiaLocal),
               };

                // Enfileira jobs apenas se houver arquivos
                if (arquivos.Any())
                {
                    var jobIdsTrafego = _jobEnqueueService.Enfileirar(arquivos);
                    _logger.LogInformation("Jobs enfileirados: {JobIds}", string.Join(", ", jobIdsTrafego));
                }
                else
                {
                    _logger.LogInformation("Nenhum arquivo para processar no exemplo");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enfileirar jobs de exemplo");
            }
        }
    }
}
