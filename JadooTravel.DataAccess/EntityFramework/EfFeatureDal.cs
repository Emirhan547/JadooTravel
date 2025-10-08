using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.DataAccess.Repositories;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.EntityFramework
{
    public class EfFeatureDal : GenericRepository<Feature>, IFeatureDal
    {
        public EfFeatureDal(JadooContext jadooContext) : base(jadooContext)
        {
        }
    }
}
