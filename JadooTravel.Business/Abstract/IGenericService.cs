using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IGenericService<TResult,TCreate,TUpdate>
    {
        Task CreateAsync(TCreate create);
        Task UpdateAsync(TUpdate update);
        Task DeleteAsync(ObjectId id);
        Task<List<TResult>> GetAllAsync();
        Task<TResult> GetByIdAsync(ObjectId id);
    }
}
