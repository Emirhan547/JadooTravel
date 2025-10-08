using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class FeatureManager : GenericManager<Feature>, IFeatureService
    {
        public FeatureManager(IGenericDal<Feature> genericDal) : base(genericDal)
        {
        }
    }
}
