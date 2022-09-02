using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace fotoTeca.Models.CitiesAndDepartments
{
    public class CitiesAndDepartmentsDAL
    {
        private readonly string _connectionStrings;

        public CitiesAndDepartmentsDAL(IConfiguration configuration/*, IDataProtectionProvider protectionProvider*/)
        {
            _connectionStrings = configuration.GetConnectionString("DefaultConnection");
            //_protector = protectionProvider.CreateProtector("nnn");

        }

        public async Task<List<CitiesResponse>> getCities()
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand("", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    var response = new List<CitiesResponse>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValue1(reader));
                        }

                    }

                    return response;
                }

            }

        }

        public CitiesResponse MapToValue1(SqlDataReader reader)
        {
            return new CitiesResponse()
            {
                idCity = (int)reader["idCity"],
                City = reader["City"].ToString(),
                idDepartment = (int)reader["Department"],
                codigo = (int)reader["codigo"],
            };
        }



        public async Task UpdateData(string respuestaNueva = "", string respuestaAntior = "", string  preguntaNueva = "", string preguntaAnterior = "")
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateDataDuel", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    if (respuestaNueva != "") { cmd.Parameters.Add(new SqlParameter("@pRespuestaNueva", respuestaNueva)); }
                    if (respuestaAntior != "") { cmd.Parameters.Add(new SqlParameter("@pRespuestaAnterior", respuestaAntior)); }
                    if (preguntaNueva != "") { cmd.Parameters.Add(new SqlParameter("@pPreguntaNueva", preguntaNueva)); }
                    if (preguntaAnterior != "") { cmd.Parameters.Add(new SqlParameter("@pPreguntaAnterior", preguntaAnterior)); }

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return;
                }

            }

        }

        public async Task skipQuestion(string cedula)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand("skipQuestion", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pCedula", cedula));

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return;
                }

            }

        }



        public async Task<Respregunta> duel(string cedula)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {
                DataResponse res = new DataResponse();

                using (SqlCommand cmd = new SqlCommand("saveDataDuel", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pCedula", cedula));
                    cmd.Parameters.Add(new SqlParameter("@pOption", 1));



                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            res.idPersona = (int)reader["idPersona"];
                            res.idPregunta = (int)reader["idPregunta"];

                        }
                    }
                    await sql.CloseAsync();

                 }

                //DataResponsePersonaPregunta resp = new DataResponsePersonaPregunta();

                //using (SqlCommand cmd = new SqlCommand("saveDataDuel", sql))
                //{
                //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //    cmd.Parameters.Add(new SqlParameter("@pOption", 2));
                //    cmd.Parameters.Add(new SqlParameter("@pidPersona", res.idPersona));
                //    cmd.Parameters.Add(new SqlParameter("@pRespuesta", respuesta));
                //    cmd.Parameters.Add(new SqlParameter("@pidPregunta", res.idPregunta + 1));


                //    await sql.OpenAsync();
                //    using (var reader = await cmd.ExecuteReaderAsync())
                //    {
                //        while (await reader.ReadAsync())
                //        {
                //            resp.idPersona = (int)reader["idPersona"];
                //        }

                //    }
                //    await sql.CloseAsync();
                //}

                Respregunta Siguientepregunta = new Respregunta();

                using (SqlCommand cmd = new SqlCommand("getPreguntas", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pCedula", cedula));

                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Siguientepregunta.pregunta = reader["nombrePregunta"].ToString();
                            Siguientepregunta.opcion1 = reader["Opcion1"].ToString();
                            Siguientepregunta.opcion2 = reader["Opcion2"].ToString();
                            Siguientepregunta.opcion3 = reader["Opcion3"].ToString();
                            Siguientepregunta.estadoCuestionario = (int)reader["estadoCuestionario"];
                            Siguientepregunta.ciudadAutor = reader["ciudadAutor"].ToString();
                            Siguientepregunta.autor = reader["autor"].ToString();
                        }

                    }
                    await sql.CloseAsync();
                }

                return Siguientepregunta; 

            }

        }


        public async Task<DataResponsePersonaPregunta> duelRespuesta(string respuesta, string cedula, int puntos)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {

                DataResponsePersonaPregunta resp = new DataResponsePersonaPregunta();

                using (SqlCommand cmd = new SqlCommand("saveDataDuel", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pOption", 2));
                    cmd.Parameters.Add(new SqlParameter("@pRespuesta", respuesta));
                    cmd.Parameters.Add(new SqlParameter("@pCedula", cedula));
                    cmd.Parameters.Add(new SqlParameter("@pPuntos", puntos));



                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            resp.puntosSumados = reader["puntosSumados"].ToString();
                            resp.puntosTotales = reader["puntosTotales"].ToString();
                        }

                    }
                    await sql.CloseAsync();
                }

                return resp;


            }

        }

        public async Task<dataPeopleDuelResponse> dataPeopleDuel(string cedula)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {

                dataPeopleDuelResponse resp = new dataPeopleDuelResponse();

                using (SqlCommand cmd = new SqlCommand("dataPeopleDuel", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pCedula", cedula));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            resp.nombre = reader["nombre"].ToString();
                            resp.ciudad = reader["ciudad"].ToString();
                            resp.puntos = reader["puntos"].ToString();
                        }

                    }
                    await sql.CloseAsync();
                }

                return resp;


            }

        }




        public async Task<List<DepartmentsResponse>> getDepartments()
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand("SP_getDepartments", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    var response = new List<DepartmentsResponse>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValue2(reader));
                        }

                    }

                    return response;
                }

            }

        }

        public DepartmentsResponse MapToValue2(SqlDataReader reader)
        {
            return new DepartmentsResponse()
            {
                idDepartment = (int)reader["idDepartment"],
                name= reader["name"].ToString(),
                code = (int)reader["code"]
            };
        }
        public async Task<List<GetDepartmentCitiesResponse>> GetDepartmentByCities(int idCity)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetDepartmentByCities", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@PidCity", idCity));

                    var response = new List<GetDepartmentCitiesResponse>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValue3(reader));
                        }

                    }

                    return response;
                }

            }

        }

        public GetDepartmentCitiesResponse MapToValue3(SqlDataReader reader)
        {
            return new GetDepartmentCitiesResponse()
            {
                idCity = (int)reader["idCity"],
                City = reader["City"].ToString(),
                idDepartment = (int)reader["idDepartment"],
                nameDepartment = reader["name"].ToString(),
            };
        }
        public async Task<List<GetDepartmentCitiesResponse>> GetCitiesByDepartment(int idDepartment)
        {
            using (SqlConnection sql = new SqlConnection(_connectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetCitiesByDepartment", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pidDepartment", idDepartment));

                    var response = new List<GetDepartmentCitiesResponse>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValue4(reader));
                        }

                    }

                    return response;
                }

            }

        }

        public GetDepartmentCitiesResponse MapToValue4(SqlDataReader reader)
        {
            return new GetDepartmentCitiesResponse()
            {
                idCity = (int)reader["idCity"],
                City = reader["City"].ToString(),
                idDepartment = (int)reader["Department"],
                nameDepartment = reader["name"].ToString(),
            };
        }

      
    }
}
