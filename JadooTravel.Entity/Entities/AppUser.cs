

using AspNetCore.Identity.MongoDbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{

    public class AppUser : MongoIdentityUser<string>
    {
        public string FullName { get; set; } = string.Empty;
    }
}
