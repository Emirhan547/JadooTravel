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
    public class MongoPartnerDal : MongoGenericDal<Partner>, IPartnerDal
    {
        public MongoPartnerDal(AppDbContext context) : base(context)
        {
        }
    }
}
