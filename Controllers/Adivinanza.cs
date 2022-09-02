using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace Microservices.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class Adivinanza : ControllerBase
    {

        private readonly CitiesAndDepartmentsDAL _repository1;

        //private readonly nodoDAL _repository2;

        //static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        //static readonly string SpreadsheetId = "1NIDdyluqcTeAXxGBdXHT2vkVuY2GpIJnFxdza7RnxL0";
        static readonly string SpreadsheetId = "13VUFEqf567wH7c2QBBTiX01pHYfU5F6f9g-q6KZ3xgc";
        static readonly string Sheet1 = "ADIVINANZA";
        static readonly string Sheet2 = "SIMULADOR";

        static SheetsService service;

        public Adivinanza(CitiesAndDepartmentsDAL repository1)
        {

            _repository1 = repository1 ?? throw new ArgumentNullException(nameof(repository1));

        }


        [HttpGet("/TraerData/{identificacion}")]
        public async Task<dataResponse> Get1(string identificacion)
        {
            GoogleCredential credential;

            dataResponse res = new dataResponse();

            using (var stream = new FileStream("Trivial.json", FileMode.Open, FileAccess.Read))
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

            Task<int> code = BuscarCelda(identificacion);

            int celda = code.Result;

            return await ReadDataByidentification(celda);

        }

        public static async Task<int> BuscarCelda(string identificacion)
        {
            var range = $"{Sheet2}!A2:A";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            int rowRead = 1;

            int result = 0;

            var response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {

                    //rowRead = row.ToString().IndexOf(pregunta);
                    rowRead++;
                    // Print columns A to F, which correspond to indices 0 and 4.

                    var cantidadDataPorfila = row.Count();

                    for (int i = 0; i < cantidadDataPorfila; i++)
                    {


                        if (row[i].ToString() == identificacion)
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
                        return rowRead;
                    }


                }

            }

            return rowRead;
        }

        public static async Task<dataResponse> ReadDataByidentification(int celda)
        {
            var range = $"{Sheet2}!A" + celda + ":N" + celda;
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            dataResponse res = new dataResponse();


            var response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    res.dato1 = (string)row[0];
                    res.dato2 = (string)row[1];
                    res.dato3 = (string)row[2];
                    res.dato4 = (string)row[3];
                    res.dato5 = (string)row[4];
                    res.dato6 = (string)row[5];
                    res.dato7 = (string)row[6];
                    res.dato8 = (string)row[7];
                    res.dato9 = (string)row[8];
                    res.dato10 = (string)row[9];
                    res.dato11 = (string)row[10];
                    res.dato12 = (string)row[11];
                    res.dato13 = (string)row[12];
                    res.dato14 = (string)row[13];
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            return res;


        } 
        public static async Task<int> ReadEntriesData(string palabra)
        {
            var range = $"{Sheet1}!A2:A";
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


        [HttpGet("/Adivinanza/{palabra}/{identificacion}/{numeroTelefono}")]
        public IActionResult Get3(string palabra, string identificacion, string numeroTelefono)
        {
            GoogleCredential credential;



            using (var stream = new FileStream("Trivial.json", FileMode.Open, FileAccess.Read))
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
                CreateEntry(identificacion, ranger.ToString(), numeroTelefono, "Gano");

                return StatusCode(200);

            }
            else
            {
                CreateEntry(identificacion, ranger.ToString(), numeroTelefono, "Perdio");

                return StatusCode(500);

            }


        }

        static void CreateEntry(string identificacion, string ranger, string numeroTelefono, string resultado)
        {
            var range = $"{Sheet1}" + "!B" + ranger + ":D" + ranger;


            var valueRange = new ValueRange();

            var oblist = new List<object>() { identificacion, numeroTelefono, resultado };
            valueRange.Values = new List<IList<object>> { oblist };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = appendRequest.Execute();
        }


        public static async Task<int> range()
        {
            var range = $"{Sheet1}!B:B";
            SpreadsheetsResource.ValuesResource.GetRequest request =
            service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            int casillaLibre = values.Count() + 1;

            return casillaLibre;
        }

        public static async Task<int> ReadEntries(string palabra)
        {
            var range = $"{Sheet1}!A2:A";
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
