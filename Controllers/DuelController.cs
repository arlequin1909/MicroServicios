using fotoTeca.Models.CitiesAndDepartments;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microservices.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DuelController : ControllerBase
    {
        private readonly CitiesAndDepartmentsDAL _repository1;

        //static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        static readonly string SpreadsheetId = "1BKMmUf8km-1CqyNUQqz7fsO5dM7rPQun-DCXC7A1TUw";
        static readonly string Sheet1 = "preguntas";
        static readonly string Sheet2 = "datosPersonas";
        static readonly string Sheet3 = "Puntos";


        static SheetsService service;

        public DuelController(CitiesAndDepartmentsDAL repository1)
        {

            _repository1 = repository1 ?? throw new ArgumentNullException(nameof(repository1));

        }

      


        [HttpGet("/ActualizarPregunta/{celda}/{pregunta}")]
        public async Task Get334(string celda, string pregunta)
        {
            GoogleCredential credential;

            using (var stream = new FileStream("Client_Secretsduel.json", FileMode.Open, FileAccess.Read))
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



            Task<string> respuestaMetod = Buscardata(celda);

            string respuestaMetodConvert = respuestaMetod.Result;


            await _repository1.UpdateData("","",pregunta, respuestaMetodConvert);


            UpdateEntry(celda, pregunta);


        }

        [HttpGet("/ActualizarRespuesta/{celda}/{respuesta}")]
        public async Task Get335(string celda,  string respuesta)
        {

            GoogleCredential credential;

            using (var stream = new FileStream("Client_Secretsduel.json", FileMode.Open, FileAccess.Read))
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



            Task<string> respuestaMetod = Buscardata(celda);

            string respuestaMetodConvert = respuestaMetod.Result;



            await _repository1.UpdateData(respuesta, respuestaMetodConvert,"", "");


            UpdateEntry(celda, respuesta);


        }

        public static async Task<string> Buscardata(string celda)
        {
            var range = $"{Sheet1}!"+ celda;
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            string result = "";

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    var cantidadDataPorfila = row.Count();

                    for (int i = 0; i < cantidadDataPorfila; i++)
                    {


                        result = row[i].ToString();
                       
                       

                    }

                }

            }

            return result;
        }

        static void UpdateEntry(string celda ,string dato)
        {
            var range = $"{Sheet1}!"+ celda;
            var valueRange = new ValueRange();

            var oblist = new List<object>() { dato };
            valueRange.Values = new List<IList<object>> { oblist };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();

        }

        [HttpGet("/traerPreguntas/{cedula}")]
        public async Task<Respregunta> Get1(string cedula)
        {
            return await _repository1.duel(cedula);
        }

        [HttpGet("/skipQuestion/{cedula}/{opcion1}/{opcion2}/{opcion3}/{respuesta}")]
        public IActionResult Get335 (string cedula, string opcion1, string opcion2, string opcion3, string respuesta)
        { 

            if (respuesta == "@opcion1")
            {
                respuesta = opcion1;
            }
            else if (respuesta == "@opcion2")
            {
                respuesta = opcion2;
            }
            else if (respuesta == "@opcion3")
            {
                respuesta = opcion3;
            }

            if (respuesta == "SALTAR PREGUNTA") 
            {
                _repository1.skipQuestion(cedula);
                return StatusCode(200);

            }
            {
                return StatusCode(500);
            }

        }

        [HttpGet("/validarEstadoPreguntas/{estadoCuestionario}")]
        public IActionResult Get1v(int estadoCuestionario)
        {
            if (estadoCuestionario == 1)
            {

                return StatusCode(200);
            }
            else 
            {

                return StatusCode(500);
            }

        }

        [HttpGet("/respuestaPregunta/{pregunta}/{opcion1}/{opcion2}/{opcion3}/{respuesta}/{cedula}/{numeroTelefono}/{nombre}/{ciudad}")]
        public async Task<DataResponsePersonaPregunta> Get11(string pregunta, string opcion1, string opcion2, string opcion3, string respuesta, string cedula, string numeroTelefono, string nombre, string ciudad)
        {
            GoogleCredential credential;

            using (var stream = new FileStream("Client_Secretsduel.json", FileMode.Open, FileAccess.Read))
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

            if (respuesta == "@opcion1")
            {
                respuesta = opcion1;
            }

            else if (respuesta == "@opcion2")
            {
                respuesta = opcion2;
            }

            else if (respuesta == "@opcion3")
            {
                respuesta = opcion3;

            }

            Task<int> casillaParaValidar = BuscarPregunta(pregunta); 

             int casillaParaValidarConvert = casillaParaValidar.Result;

            Task<int> infoResultado = ValidarRespuesta(respuesta, casillaParaValidarConvert);

            int infoResultadoConvert = infoResultado.Result;

            var data = await _repository1.dataPeopleDuel(cedula);

            if (data.puntos == "")
            {
                data.puntos = "0";
            }

            dataPeopleDuelResponse resp = new dataPeopleDuelResponse();

            int puntosTotales = Convert.ToInt32(data.puntos.ToString());

            int total = puntosTotales + 5; 

            if (infoResultadoConvert == 1)
            {
                CreateEntry(nombre, ciudad, cedula, numeroTelefono, "Correcto", "5", total.ToString());
                return await _repository1.duelRespuesta(respuesta, cedula, 5);
            }
            else
            {
                CreateEntry(nombre, ciudad, cedula, numeroTelefono, "Incorrecto", "0",data.puntos);
                return await _repository1.duelRespuesta(respuesta, cedula, 0);
            }

            //CreateEntry();

        }
        public static async Task<int> BuscarPregunta(string pregunta)
        {
            var range = $"{Sheet1}!A2:A";
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


                            if (row[i].ToString() == pregunta)
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

            return result;
        }


   


        public static async Task<int> ValidarRespuesta(string respuesta, int casillaParaValidarConvert)
        {
            var range = $"{Sheet1}!B" + casillaParaValidarConvert.ToString();
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

                        if (row[i].ToString() == respuesta)
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

        static void CreateEntry(string nombre,string ciudad,string identificacion, string numeroTelefono, string resultado, string puntaje, string puntajeTotal )
        {
            var range = $"{Sheet2}!A:H";



            string datetime = DateTime.Now.AddHours(-5).ToString();

            var valueRange = new ValueRange();

            var oblist = new List<object>() { nombre,ciudad, identificacion,numeroTelefono,resultado,puntaje,datetime, puntajeTotal };
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


    }

}
    
