using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.DataAccess.Repositories;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.EntityFramework
{
    public class EfDestinationDal : GenericRepository<Destination>, IDestinationDal
    {
        public EfDestinationDal(JadooContext jadooContext) : base(jadooContext)
        {
        }
    }
}
