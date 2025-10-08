using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IGenericService<T> where T : class
    {
        Task TCreateAsync(T entity);
        Task TUpdateAsync(T entity);
        Task TDeleteAsync(ObjectId id);
        Task<List<T>> TGetAllListAsync();
        Task<T> TGetByIdAsync(ObjectId id);
    }
}
