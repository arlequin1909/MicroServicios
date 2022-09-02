using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NegronWebApi.Models
{
    public class UserInfo
    {
        [Required(ErrorMessage = "El Email es obligatorio")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El Password es obligatorio")]
        public string Password { get; set; }
    }
}
