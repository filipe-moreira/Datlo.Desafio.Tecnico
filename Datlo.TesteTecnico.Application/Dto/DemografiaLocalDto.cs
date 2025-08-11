namespace Datlo.TesteTecnico.Application.Dto
{
    public sealed record DemografiaLocalDto(DateTime DataRegistro, double Lat,
        double Lon, int Populacao, double RendaMedia, string FaixaEtariaPredominante);
}
