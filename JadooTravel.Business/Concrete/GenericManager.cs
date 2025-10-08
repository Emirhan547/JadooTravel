using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class GenericManager<T> : IGenericService<T> where T : class
    {
        private readonly IGenericDal<T> _genericDal;

        public GenericManager(IGenericDal<T> genericDal)
        {
            _genericDal = genericDal;
        }

        public async Task<List<T>> TGetAllListAsync()
        {
            return await _genericDal.GetAllListAsync();
        }

        public async Task<T> TGetByIdAsync(ObjectId id)
        {
           return await _genericDal.GetByIdAsync(id);
        }


        public async Task TCreateAsync(T entity)
        {
            await _genericDal.CreateAsync(entity);
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            await _genericDal.DeleteAsync(id);
        }

        public async Task TUpdateAsync(T entity)
        {
            await _genericDal.UpdateAsync(entity);
        }
    }
}
