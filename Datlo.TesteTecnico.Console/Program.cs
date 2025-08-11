var diretorio_demografiaLocal_c = $@"C:\Users\xilip\OneDrive\Área de Trabalho\csvzao\cliente_c_demografia_local_{Guid.NewGuid()}.csv";
GeradorDeArquivos.GerarCsvFake_ClienteC_DemografiaLocal(1_000_000, diretorio_demografiaLocal_c, ";");

var diretorio_demografiaLocal_d = $@"C:\Users\xilip\OneDrive\Área de Trabalho\csvzao\cliente_d_demografia_local_{Guid.NewGuid()}.csv";
GeradorDeArquivos.GerarCsvFake_ClienteD_DemografiaLocal(1_000_000, diretorio_demografiaLocal_d, ";");

var diretorio_trafegoPessoas_c = $@"C:\Users\xilip\OneDrive\Área de Trabalho\csvzao\cliente_c_trafego_pessoas_{Guid.NewGuid()}.csv";
GeradorDeArquivos.GerarCsvFake_ClienteC_TrafegoPessoa(1_000_000, diretorio_trafegoPessoas_c, ";");

var diretorio_trafegoPessoas_d = $@"C:\Users\xilip\OneDrive\Área de Trabalho\csvzao\cliente_d_trafego_pessoas_{Guid.NewGuid()}.csv";
GeradorDeArquivos.GerarCsvFake_ClienteD_TrafegoPessoa(1_000_000, diretorio_trafegoPessoas_d, ";");

