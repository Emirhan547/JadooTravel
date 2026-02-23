using JadooTravel.Entity.Entities;

namespace JadooTravel.DataAccess.Abstract
{
    public interface IUserFavoriteDal : IGenericDal<UserFavorite>
    {
        Task<bool> ExistsAsync(string userId, string destinationId);
        Task<long> CountByUserIdAsync(string userId);
        Task<bool> DeleteByIdAndUserIdAsync(string favoriteId, string userId);
        Task<List<UserFavorite>> GetByUserIdAsync(string userId);
    }
}