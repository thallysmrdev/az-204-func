using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dio.AZ_204
{
    public class FnValidacaoCPF
    {
        private readonly ILogger<FnValidacaoCPF> _logger;

        public FnValidacaoCPF(ILogger<FnValidacaoCPF> logger)
        {
            _logger = logger;
        }

        [Function("FnValidacaoCPF")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("Validação de CPF - Inicio");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data == null) {                
                return new BadRequestObjectResult("Por favor, informe o CPF");
            }

            string cpf = data?.cpf;

            if (!ValidarCPF(cpf)) {
                return new BadRequestObjectResult("CPF inválido !");
            }
            
            _logger.LogInformation("Validação de CPF - Fim");
            return new OkObjectResult("CPF válido !");
        }

        public static bool ValidarCPF(string cpf)
	    {
            if (string.IsNullOrEmpty(cpf))
            {
                return false;
            }

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
            {
                return false;
            }

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for(int i=0; i<9; i++) 
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            resto = soma % 11;

            if ( resto < 2 )
            {
                resto = 0;
            }
            else 
            {
                resto = 11 - resto;
            }

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for(int i=0; i<10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;

            if (resto < 2)
            {
            resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }
    }
}
