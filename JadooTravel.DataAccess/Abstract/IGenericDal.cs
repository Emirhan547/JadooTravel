using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.Abstract
{
    public interface IGenericDal<TEntity> where TEntity : class
    {
        Task<TEntity>GetByIdAsync(string id);
        Task<IList<TEntity>> GetAllAsync();
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(string id);
    }
}
    