using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.Entity.Entities;
using MongoDB.Driver;

namespace JadooTravel.DataAccess.Concrete
{
    public class MongoGenericDal<TEntity>
        : IGenericDal<TEntity>
        where TEntity : BaseEntity
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoGenericDal(AppDbContext context)
        {
            _collection = context.GetCollection<TEntity>();
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await _collection.ReplaceOneAsync(
                x => x.Id == entity.Id,
                entity);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}