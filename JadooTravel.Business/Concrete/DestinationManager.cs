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
    public class DestinationManager : GenericManager<Destination>, IDestinationService
    {
        public DestinationManager(IGenericDal<Destination> genericDal) : base(genericDal)
        {
        }
    }
}
