using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.Abstract
{
    public interface IGenericDal<T> where T : class
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(ObjectId id);
        Task<List<T>> GetAllListAsync();
        Task<T> GetByIdAsync(ObjectId id);
    }
}
