using JadooTravel.Dto.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Abstract
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(UpdateProfileDto updateProfileDto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> AddFavoriteAsync(string userId, string destinationId, string cityCountry, string imageUrl, decimal price);
        Task<bool> RemoveFavoriteAsync(string userId, string favoriteId);
        Task<List<UserFavoriteDto>> GetFavoritesAsync(string userId);
    }
}
