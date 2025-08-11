using Amazon.S3.Util;
using Datlo.TesteTecnico.Domain.Models;
using Datlo.TesteTecnico.Worker.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Datlo.TesteTecnico.Worker.Controllers
{
    [ApiController]
    [Route("events/s3")]
    public class S3EventsController : ControllerBase
    {
        private readonly IBackgroundJobClient _jobs;

        public S3EventsController(IBackgroundJobClient jobs)
        {
            _jobs = jobs;
        }

        [HttpPost]
        public IActionResult OnS3Event([FromBody] S3EventNotification evt)
        {
            // apenas para exemplificar
            foreach (var r in evt.Records)
            {
                var arquivo = new ArquivosS3("", "", "", 
                                             TipoArquivo.Xml, 
                                             ModeloArquivo.TrafegoPessoas);

                _jobs.Enqueue<IProcessamentoJob>(j =>
                    j.ExecutarAsync(arquivo, CancellationToken.None));
            }
            return Ok();
        }
    }

}
