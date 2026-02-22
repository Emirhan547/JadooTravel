using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.Concrete
{
    public class MongoCtaDal : MongoGenericDal<Cta>, ICtaDal
    {
        public MongoCtaDal(AppDbContext context) : base(context)
        {
        }
    }
}
