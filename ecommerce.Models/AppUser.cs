using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class AppUser :IdentityUser
    {
        public string City { get; set; }
       
        public string Branch { get; set; }

        [NotMapped]
        public string Role { get; set; }
      

    }
}
