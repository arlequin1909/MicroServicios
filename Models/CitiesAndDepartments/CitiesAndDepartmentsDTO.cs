using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fotoTeca.Models.CitiesAndDepartments
{
    public class CitiesResponse
    {
        public int idCity { get; set; }
        public string City { get; set; }
        public int idDepartment { get; set; }
        public int codigo { get; set; }


    }

    public class dataResponse
    {
        public string dato1 { get; set; }
        public string dato2 { get; set; }
        public string dato3 { get; set; }
        public string dato4 { get; set; }
        public string dato5 { get; set; }
        public string dato6 { get; set; }
        public string dato7 { get; set; }
        public string dato8 { get; set; }
        public string dato9 { get; set; }
        public string dato10 { get; set; }
        public string dato11 { get; set; }
        public string dato12 { get; set; }
        public string dato13 { get; set; }
        public string dato14 { get; set; }


    }

    public class updateResponse
    {
        public string response { get; set; }

    }
    public class DepartmentsResponse
    {
        public int idDepartment { get; set; }
        public string name { get; set; }
        public int code { get; set; }

    }

    public class DataResponse
    {
        public int idPersona { get; set; }
        public int idPregunta { get; set; }
   
    }

    public class DataResponsePersonaPregunta
    {
        public string puntosSumados { get; set; }
        public string puntosTotales { get; set; }

    }


    public class dataPeopleDuelResponse
    {
        public string nombre { get; set; }
        public string ciudad { get; set; }
        public string puntos { get; set; }

    }

    public class Respregunta
    {
        public string pregunta { get; set; }
        public string opcion1 { get; set; }
        public string opcion2 { get; set; }
        public string opcion3 { get; set; }
        public int estadoCuestionario { get; set; }
        public string autor { get; set; }
        public string ciudadAutor { get; set; }



    }
    public class GetDepartmentCitiesResponse
    {
        public int idCity { get; set; }
        public string City { get; set; }
        public int idDepartment { get; set; }
        public string nameDepartment { get; set; }
    }


}
