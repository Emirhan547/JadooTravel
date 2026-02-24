using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.UserDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace JadooTravel.Business.Concrete
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        private readonly IBookingDal _bookingDal;
        private readonly IUserFavoriteDal _userFavoriteDal;

        public UserProfileService(
            UserManager<AppUser> userManager,
            IMapper mapper,
        IBookingDal bookingDal,
            IUserFavoriteDal userFavoriteDal)
        {
            _userManager = userManager;
            _mapper = mapper;
            _bookingDal = bookingDal;
            _userFavoriteDal = userFavoriteDal;
        }

        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var bookingCount = await _bookingDal.CountByUserIdAsync(userId);
            var favoriteCount = await _userFavoriteDal.CountByUserIdAsync(userId);
            var favorites = await _userFavoriteDal.GetByUserIdAsync(userId);

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address ?? string.Empty,
                City = user.City ?? string.Empty,
                Country = user.Country ?? string.Empty,
                ProfileImageUrl = user.ProfileImageUrl ?? "/public/assets/img/default-avatar.png",
                CreatedDate = DateTime.UtcNow,
                TotalBookings = (int)bookingCount,
                TotalFavorites = (int)favoriteCount,
                FavoriteDestinations = _mapper.Map<List<UserFavoriteDto>>(favorites)
            };
        }
        

        public async Task<bool> UpdateProfileAsync(UpdateProfileDto updateProfileDto)
        {
            var user = await _userManager.FindByIdAsync(updateProfileDto.Id);
            if (user == null)
                return false;

            user.FullName = updateProfileDto.FullName ?? user.FullName;
            user.PhoneNumber = updateProfileDto.PhoneNumber ?? user.PhoneNumber;
            user.Address = updateProfileDto.Address ?? user.Address;
            user.City = updateProfileDto.City ?? user.City;
            user.Country = updateProfileDto.Country ?? user.Country;
            user.ProfileImageUrl = updateProfileDto.ProfileImageUrl ?? user.ProfileImageUrl;

            if (!string.IsNullOrWhiteSpace(updateProfileDto.Email) && updateProfileDto.Email != user.Email)
            {
                user.Email = updateProfileDto.Email;
                user.UserName = updateProfileDto.Email;
            }
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                throw new Exception("Yeni şifreler eşleşmiyor");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            return result.Succeeded;
        }

        public async Task<bool> AddFavoriteAsync(string userId, string destinationId, string cityCountry, string imageUrl, decimal price)
        {
            if (await _userFavoriteDal.ExistsAsync(userId, destinationId))
                return false;

            var favorite = new UserFavorite
            {
                UserId = userId,
                DestinationId = destinationId,
                CityCountry = cityCountry,
                ImageUrl = imageUrl,
                Price = price,
                CreatedDate = DateTime.UtcNow
            };

            await _userFavoriteDal.CreateAsync(favorite);
            return true;
        }

        public async Task<bool> ToggleFavoriteAsync(string userId, string destinationId, string cityCountry, string imageUrl, decimal price)
        {
            var exists = await _userFavoriteDal.ExistsAsync(userId, destinationId);
            if (exists)
                return await _userFavoriteDal.DeleteByUserIdAndDestinationIdAsync(userId, destinationId);

            return await AddFavoriteAsync(userId, destinationId, cityCountry, imageUrl, price);
        }
        public async Task<bool> RemoveFavoriteAsync(string userId, string favoriteId)
       => await _userFavoriteDal.DeleteByIdAndUserIdAsync(favoriteId, userId);

        public async Task<List<UserFavoriteDto>> GetFavoritesAsync(string userId)
    
            => _mapper.Map<List<UserFavoriteDto>>(await _userFavoriteDal.GetByUserIdAsync(userId));

        public async Task<List<FavoriteDestinationStatDto>> GetFavoritesByDestinationAsync()
        {
            var favorites = await _userFavoriteDal.GetAllAsync();

            return favorites
                .GroupBy(x => new { x.DestinationId, x.CityCountry })
                .Select(g => new FavoriteDestinationStatDto
                {
                    DestinationId = g.Key.DestinationId,
                    CityCountry = g.Key.CityCountry,
                    FavoriteCount = g.Count()
                })
                .OrderByDescending(x => x.FavoriteCount)
                .Take(8)
                .ToList();
        }
    }


}