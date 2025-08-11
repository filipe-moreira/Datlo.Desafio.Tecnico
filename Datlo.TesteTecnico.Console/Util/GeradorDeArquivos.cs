using System.Globalization;
using System.Text;
using System.Text.Json;

public static class GeradorDeArquivos
{
    public static void GerarJsonFake_ClienteA_TrafegoPessoa(int quantidade, string caminhoArquivo)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

        var options = new JsonWriterOptions
        {
            Indented = false, // deixe true se quiser legível
            SkipValidation = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var random = new Random();

        using var stream = new FileStream(caminhoArquivo, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new Utf8JsonWriter(stream, options);

        writer.WriteStartArray();

        for (int i = 0; i < quantidade; i++)
        {
            var data = DateTime.UtcNow.AddMinutes(-random.Next(0, 60 * 24 * 30)); // até 30 dias atrás
            var dataStr = data.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // string, como seu mapper espera

            var qtd = random.Next(0, 1000);
            var lat = Math.Round(-23.5505 + random.NextDouble() * 0.1, 6);
            var lon = Math.Round(-46.6333 + random.NextDouble() * 0.1, 6);

            writer.WriteStartObject();
            writer.WriteString("data", dataStr);
            writer.WriteNumber("pessoas_estimada", qtd);
            writer.WriteNumber("latitude", lat);
            writer.WriteNumber("longitude", lon);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.Flush();

        Console.WriteLine($"Arquivo JSON (ClienteA) gerado com sucesso: {caminhoArquivo}");
    }
    public static void GerarJsonFake_ClienteB_TrafegoPessoa(int quantidade, string caminhoArquivo)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

        var options = new JsonWriterOptions
        {
            Indented = false,
            SkipValidation = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var random = new Random();

        using var stream = new FileStream(caminhoArquivo, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new Utf8JsonWriter(stream, options);

        writer.WriteStartArray();

        for (int i = 0; i < quantidade; i++)
        {
            var data = DateTime.UtcNow.AddMinutes(-random.Next(0, 60 * 24 * 30)); // até 30 dias atrás
            var dataIso = data.ToString("O", CultureInfo.InvariantCulture); // ISO 8601 -> GetDateTime() funciona

            var qtd = random.Next(0, 1000);
            var lat = Math.Round(-23.5505 + random.NextDouble() * 0.1, 6);
            var lon = Math.Round(-46.6333 + random.NextDouble() * 0.1, 6);

            writer.WriteStartObject();             // { 
            writer.WritePropertyName("properties");
            writer.WriteStartObject();             // "properties": {
            writer.WriteString("data_registro", dataIso);
            writer.WriteNumber("qtd_estimada_pessoas", qtd);
            writer.WriteNumber("latitude", lat);
            writer.WriteNumber("longitude", lon);
            writer.WriteEndObject();               // }
            writer.WriteEndObject();               // }
        }

        writer.WriteEndArray();
        writer.Flush();

        Console.WriteLine($"Arquivo JSON (ClienteB) gerado com sucesso: {caminhoArquivo}");
    }
    public static void GerarCsvFake_ClienteC_TrafegoPessoa(int quantidade, string caminhoArquivo, string delimitador = ";")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

        using var writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);

        // Cabeçalho
        writer.WriteLine($"data_registro{delimitador}qtd_estimada_pessoas{delimitador}latitude{delimitador}longitude");

        var random = new Random();

        for (int i = 0; i < quantidade; i++)
        {
            var dataRegistro = DateTime.UtcNow
                .AddMinutes(-random.Next(0, 60 * 24 * 30)) // até 30 dias atrás
                .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var qtd = random.Next(0, 1000); // quantidade estimada
            var lat = Math.Round(-23.5505 + random.NextDouble() * 0.1, 6); // próximo a SP
            var lon = Math.Round(-46.6333 + random.NextDouble() * 0.1, 6); // próximo a SP

            writer.WriteLine($"{dataRegistro}{delimitador}{qtd}{delimitador}{lat.ToString(CultureInfo.InvariantCulture)}{delimitador}{lon.ToString(CultureInfo.InvariantCulture)}");
        }

        Console.WriteLine($"Arquivo gerado com sucesso: {caminhoArquivo}");
    }
    public static void GerarCsvFake_ClienteD_TrafegoPessoa(int quantidade, string caminhoArquivo, string delimitador = "|")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

        using var writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);

        // Cabeçalho
        writer.WriteLine($"Latitude{delimitador}" +
                         $"Longitude{delimitador}" +
                         $"Data do Evento{delimitador}" +
                         $"Quantidade de Pessoas{delimitador}");

        var random = new Random();

        for (int i = 0; i < quantidade; i++)
        {
            var dataRegistro = DateTime.UtcNow
                .AddMinutes(-random.Next(0, 60 * 24 * 30)) // até 30 dias atrás
                .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var qtd = random.Next(0, 1000); // quantidade estimada
            var lat = Math.Round(-23.5505 + random.NextDouble() * 0.1, 6); // próximo a SP
            var lon = Math.Round(-46.6333 + random.NextDouble() * 0.1, 6); // próximo a SP
                                                                           
            writer.WriteLine($"{lat.ToString(CultureInfo.InvariantCulture)}{delimitador}" +
                             $"{lon.ToString(CultureInfo.InvariantCulture)}{delimitador}" +
                             $"{dataRegistro}{delimitador}" +
                             $"{qtd}");
        }

        Console.WriteLine($"Arquivo gerado com sucesso: {caminhoArquivo}");
    }

    public static void GerarCsvFake_ClienteC_DemografiaLocal(int quantidade, string caminhoArquivo, string delimitador = ";")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

        using var writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);

        // Cabeçalho seguindo o mapeamento
        writer.WriteLine($"data_registro{delimitador}" +
                         $"latitude{delimitador}" +
                         $"longitude{delimitador}" +
                         $"populacao{delimitador}" +
                         $"renda_media{delimitador}" +
                         $"faixa_etaria_predominante");

        var random = new Random();
        var faixasEtarias = new[]
        {
        "0-14", "15-24", "25-34", "35-44", "45-59", "60+"
    };

        for (int i = 0; i < quantidade; i++)
        {
            var dataRegistro = DateTime.UtcNow
                .AddMinutes(-random.Next(0, 60 * 24 * 365)) // até 1 ano atrás
                .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var lat = Math.Round(-23.5505 + random.NextDouble() * 0.1, 6); // próximo a SP
            var lon = Math.Round(-46.6333 + random.NextDouble() * 0.1, 6);
            var populacao = random.Next(500, 50000); // habitantes
            var rendaMedia = Math.Round(random.NextDouble() * 10000, 2); // em moeda local
            var faixaEtaria = faixasEtarias[random.Next(faixasEtarias.Length)];

            writer.WriteLine($"{dataRegistro}{delimitador}" +
                             $"{lat.ToString(CultureInfo.InvariantCulture)}{delimitador}" +
                             $"{lon.ToString(CultureInfo.InvariantCulture)}{delimitador}" +
                             $"{populacao}{delimitador}" +
                             $"{rendaMedia.ToString(CultureInfo.InvariantCulture)}{delimitador}" +
                             $"{faixaEtaria}");
        }

        Console.WriteLine($"Arquivo CSV (ClienteC - DemografiaLocal) gerado com sucesso: {caminhoArquivo}");
    }
    public static void GerarCsvFake_ClienteD_DemografiaLocal(int quantidade, string caminhoArquivo, string delimitador = "|")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

        using var writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);

        // Cabeçalho seguindo a ordem da propriedade ColunasCsvDemografiaLocal do ClienteD
        writer.WriteLine($"População{delimitador}" +
                         $"Renda Média{delimitador}" +
                         $"Faixa Etária Predominante{delimitador}" +
                         $"Data de Registro{delimitador}" +
                         $"Latitude{delimitador}" +
                         $"Longitude");

        var random = new Random();
        var faixasEtarias = new[]
        {
        "0-14", "15-24", "25-34", "35-44", "45-59", "60+"
    };

        for (int i = 0; i < quantidade; i++)
        {
            var populacao = random.Next(500, 50000); // habitantes
            var rendaMedia = Math.Round(random.NextDouble() * 10000, 2); // moeda local
            var faixaEtaria = faixasEtarias[random.Next(faixasEtarias.Length)];

            var dataRegistro = DateTime.UtcNow
                .AddMinutes(-random.Next(0, 60 * 24 * 365)) // até 1 ano atrás
                .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var lat = Math.Round(-23.5505 + random.NextDouble() * 0.1, 6);
            var lon = Math.Round(-46.6333 + random.NextDouble() * 0.1, 6);

            writer.WriteLine($"{populacao}{delimitador}" +
                             $"{rendaMedia.ToString(CultureInfo.InvariantCulture)}{delimitador}" +
                             $"{faixaEtaria}{delimitador}" +
                             $"{dataRegistro}{delimitador}" +
                             $"{lat.ToString(CultureInfo.InvariantCulture)}{delimitador}" +
                             $"{lon.ToString(CultureInfo.InvariantCulture)}");
        }

        Console.WriteLine($"Arquivo CSV (ClienteD - DemografiaLocal) gerado com sucesso: {caminhoArquivo}");
    }
}
