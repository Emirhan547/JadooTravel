using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.Entity.Entities;

namespace JadooTravel.DataAccess.Concrete
{
    public class MongoBookingDal
        : MongoGenericDal<Booking>, IBookingDal
    {
        public MongoBookingDal(AppDbContext context)
            : base(context)
        {
        }
    }
}