namespace Datlo.TesteTecnico.Worker.Options
{
    public class ProcessamentoJobOptions
    {
        public int TimeoutMinutos { get; init; } = 30;
        public int MaxTentativas { get; init; } = 3;
        public int[] DelayEntreRetrySegundos { get; init; } = new[] { 30, 120, 300 };
        public TimeoutPorTipoArquivoOptions TimeoutPorTipoArquivo { get; init; } = new();
    }

    public class TimeoutPorTipoArquivoOptions
    {
        public int TrafegoPessoas { get; init; } = 20;
        public int DemografiaLocal { get; init; } = 25;
        public int Default { get; init; } = 30;
    }
}