using fotoTeca.Models.CitiesAndDepartments;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SheetsQuickstart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fotoTeca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ServicesController : ControllerBase
    {
        private readonly CitiesAndDepartmentsDAL _repository1;

        //private readonly nodoDAL _repository2;

        //static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        //static readonly string SpreadsheetId = "1NIDdyluqcTeAXxGBdXHT2vkVuY2GpIJnFxdza7RnxL0";
        static readonly string SpreadsheetId = "1xuBnC7dOTrTn9Altot6j2JFajfcnS4a4VHUz_--zcbg";
        //static readonly string Sheet = "prueba";
        static readonly string Sheet = "palabras";

        static SheetsService service;

        public ServicesController(CitiesAndDepartmentsDAL repository1)
        {

            _repository1 = repository1 ?? throw new ArgumentNullException(nameof(repository1));

        }
        //[HttpGet("/getCities")]
        //public async Task<List<CitiesResponse>> Get1()
        //{
        //    return await _repository1.getCities();

        //}
        //[HttpGet("/getDepartments")]
        //public async Task<List<DepartmentsResponse>> Get2()
        //{
        //    return await _repository1.getDepartments();

        //}
        //[HttpGet("/GetDepartmentByCities/{idCity}")]
        //public async Task<List<GetDepartmentCitiesResponse>> Get3(int idCity)
        //{
        //    return await _repository1.GetDepartmentByCities(idCity);

        //}
        //[HttpGet("/GetCitiesByDepartment/{idDepartment}")]
        //public async Task<List<GetDepartmentCitiesResponse>> Get4(int idDepartment)
        //{
        //    return await _repository1.GetCitiesByDepartment(idDepartment);

        //}

        //[HttpGet("/Nodow1/{Cedula}")]
        //public async Task<string> GetCedula(string Cedula)
        //{
        //    GoogleCredential credential;

        //    //var json = "{\"private_key_id\":\"xxxxxx\",\"private_key\":\"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDcmuyQC8rwWdPQ\\nmIdksgzSJbVWTU5MeUxy+HAap3yut9wR/L6KGMJ4FBYcsPmXN5gQAhErybavGoZG\\nfS1X1/PCpPVpTCA4749K8gbvuZg1JEIAqMtmHiBBrJj5l8eiekQc8pd7Pq35H4wi\\nJYXAJGwggPcttkLBRi0xZzd+jdwL1st+7zRt8nMao/xFibInBBvKwb/gP4mJxlQg\\nnRdGO6zgMk+PLTcA5C+gFyPA4SdkylrLib5CJO9123FgcfTJZJTukeHo1v0EfU+4\\n3bK8HBZnOFa4DHH4mXhkhgYMjibv4Sr/WCEoomJJwNN04SbUEdyhgpM2rZ3cvx+4\\nsmB0SQflAgMBAAECggEAXZ100+/dL7++zh9cHVQdcrRDzprBplw3H/bjg7wdgftN\\n7Wgm5214YQKNG6HSWORjqC9oX/+agZYs8w69xjBDJg9ggU2nwuGOGky4utQ0jiCT\\nzbnTjsMsBxKaXBiXxBBEhVBBDjDcHQLRMdBggNgz9lskCYb1rxT7qqJVf2PtxCuZ\\nuxw3whLMRHXvKosER12sMQgGB/0+Nk86GWCqPigpfu7Ec92V0ffcSUaq3gjIUD54\\n67TduTWaRDQNB+j2yQsWQZnqRv+TvIXOjinAI+pPbvCUovtiTSZAoz3EalsiXQ0l\\nUqDVx26uzEJqhB2kzvAeApuW2Nd5EPxUnf48c4xh4QKBgQDw01mEChWyENV5CBKU\\nMSfY0rpAPtq7ahHRR458ZKtITDBlqiZLMjydI65Rr1XxpQ3pJZALObMdUhbvCDfm\\nu4BY/lCCt+hcdt9IICvVZsgXgvb6M+Fj2IbYZcAnOm4T1Z1D3I+pW5NdK2ALQRiK\\nWsGINOqWCB9WRd7nhmb/XwWyjQKBgQDqgWht5laDuLMc4qpj9finY4qmk57eT3KG\\npzbVlT3h7kv7j/j6e+6o9psrqdf1PXpu9XZi3bPtPbH1fX9x5pZgJQRMP4FGOURY\\nQDkJfiOOSN/8Vl0senqkscT7DSbe2BqyqQlSlTB4BBF29p1wxb5Wz5HH2BvYE2zI\\ni9B4WJcAuQKBgADnajCasRYoBgUcSKWRwaqIr/ZJxhxp+4Mjl59T6WiuEIhxKQ+j\\nMqMMXT0lQVdU3UaAw5enMcrsYfWnvD37ejHbUoYLFq4yLAhjRobYieu8rByoUTJE\\nv8zUJPKAv6UHaj20+D0UgOsanJOuPN9YE93lBPRnN2blgD6yPHS88JKJAoGABFyh\\n16F4LH0L/9aLes6BcIOeeZi3VMU/iRelInXjL8eh7CzyYZ5agxQLMNW46ZvaIiQ4\\nroAXL6t9GubZrwGt/F3T5aMswWShS87uAKoy+RuL5wKoOwKQM24HDvBgr7ZvULFq\\nNfoGa8UPmhneNdHHx4+W05PGeM9rr5NCLmrfbCkCgYA0nMvEDIJvU3KA3S1cQ3fs\\nVopRJwqRIFFL1cHTWaEyIsxEh6i/zAUc/habK82dN3/ZDn/XvWY14k7VZPsSdDC9\\noVlQj2z8DVO2K99Oxyh0VlthtecW8exjzkIPJL4srOSl/dooQZS/7ZZyaRQU/BLI\\nMdzKHlUKKXWcUU+Ko8W4+w==\\n-----END PRIVATE KEY-----\\n\",\"client_email\":\"dddddd-2a1f881e7rabfkt2eb1p84aisg30pedg@developer.gserviceaccount.com\",\"client_id\":\"ddddd-2a1f881e7rabfkt2eb1p84aisg30pedg.apps.googleusercontent.com\",\"type\":\"service_account\"}";

        //    //dynamic jsonObject = JsonConvert.DeserializeObject(json);

        //    using (var stream = new FileStream("Client_Secrets.json", FileMode.Open, FileAccess.Read))
        //    {
        //        credential = GoogleCredential.FromStream(stream)
        //            .CreateScoped(Scopes);
        //    }

        //    // Create Google Sheets API service.
        //    service = new SheetsService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName,
        //    });

        //    var ranger = "!A:A";

        //    CreateEntry(Cedula,ranger);
        //    //ReadEntries();

        //    return "hola";
        //}

        
        [HttpGet("/Nodo/{palabra}/{identificacion}/{numeroTelefono}")]
        public IActionResult Get3(string palabra,string identificacion, string numeroTelefono)
        {
            GoogleCredential credential;



            using (var stream = new FileStream("Client_Secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            range();

            var ranger = range().Result;


            Task<int> code = ReadEntries(palabra);

            int codeConvert = code.Result;

            var resultado = "ninguno";
            


            if (codeConvert == 1)
            {
                CreateEntry(identificacion, ranger.ToString(),numeroTelefono,"Gano");

                return StatusCode(200);

            }
            else
            {
                CreateEntry(identificacion, ranger.ToString(),numeroTelefono,"Perdio");

                return StatusCode(500);

            }


        }

        static void CreateEntry(string identificacion, string ranger, string numeroTelefono, string resultado)
        {
            var range = $"{Sheet}" + "!B" + ranger + ":D" + ranger;


            var valueRange = new ValueRange();

            var oblist = new List<object>() { identificacion, numeroTelefono, resultado  };
            valueRange.Values = new List<IList<object>> { oblist };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = appendRequest.Execute();
        }


        public static async Task<int> range()
        {
            var range = $"{Sheet}!B:B";
            SpreadsheetsResource.ValuesResource.GetRequest request =
            service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            int casillaLibre = values.Count() + 1;

            return casillaLibre;
        }

        public static async Task<int> ReadEntries(string palabra)
        {
            var range = $"{Sheet}!A2:A";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            int result = 0; 

            var response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    // Print columns A to F, which correspond to indices 0 and 4.
                    var cantidadDataPorfila = row.Count();

                    for (int i = 0; i < cantidadDataPorfila; i++)
                    {
                        if (palabra.Contains(row[i].ToString()))
                        {
                            result = 1;
                            break;
                            
                        }
                        else 
                        {
                            result = 2;
                            Console.Write("No data found.");
                        }

                    }

                    if (result == 1)
                    {
                        //break;
                        return result;
                    }


                }
            }

            return result;
        }


        

    }
}
