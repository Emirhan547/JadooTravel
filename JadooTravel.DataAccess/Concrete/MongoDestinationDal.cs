using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.Concrete
{
    public class MongoDestinationDal : MongoGenericDal<Destination>, IDestinationDal
    {
        public MongoDestinationDal(AppDbContext context) : base(context)
        {
        }
    }
}
