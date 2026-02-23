using AutoMapper;
using JadooTravel.Business.Abstract;
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
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;

        public UserProfileService(
            UserManager<AppUser> userManager,
            IMapper mapper,
            IMongoClient mongoClient)
        {
            _userManager = userManager;
            _mapper = mapper;
            _mongoClient = mongoClient;
            _mongoDatabase = mongoClient.GetDatabase("JadooTravelDb");
        }

        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return null;

                var bookingCollection = _mongoDatabase.GetCollection<dynamic>("Bookings");
                var bookingFilter = Builders<dynamic>.Filter.Eq("UserId", userId);
                var bookingCount = await bookingCollection.CountDocumentsAsync(bookingFilter);

                var favoriteCollection = _mongoDatabase.GetCollection<dynamic>("UserFavorites");
                var favoriteFilter = Builders<dynamic>.Filter.Eq("UserId", userId);
                var favoriteCount = await favoriteCollection.CountDocumentsAsync(favoriteFilter);

                var profileDto = new UserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address ?? "",
                    City = user.City ?? "",
                    Country = user.Country ?? "",
                    ProfileImageUrl = user.ProfileImageUrl ?? "/public/assets/img/default-avatar.png",
                    CreatedDate = user.Id != null ? DateTime.UtcNow : DateTime.UtcNow,
                    TotalBookings = (int)bookingCount,
                    TotalFavorites = (int)favoriteCount
                };

                return profileDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Profil yüklenirken hata oluştu: {ex.Message}");
            }
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileDto updateProfileDto)
        {
            try
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

                // Email değişirse güncelle
                if (!string.IsNullOrWhiteSpace(updateProfileDto.Email) &&
                    updateProfileDto.Email != user.Email)
                {
                    user.Email = updateProfileDto.Email;
                    user.UserName = updateProfileDto.Email;
                }

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new Exception($"Profil güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                    throw new Exception("Yeni şifreler eşleşmiyor");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    changePasswordDto.CurrentPassword,
                    changePasswordDto.NewPassword
                );

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new Exception($"Şifre değiştirilirken hata oluştu: {ex.Message}");
            }
        }

        public async Task<bool> AddFavoriteAsync(string userId, string destinationId,
            string cityCountry, string imageUrl, decimal price)
        {
            try
            {
                var favoriteCollection = _mongoDatabase.GetCollection<UserFavorite>("UserFavorites");

                // Zaten favorilere eklenmiş mi kontrol et
                var filter = Builders<UserFavorite>.Filter.And(
                    Builders<UserFavorite>.Filter.Eq(x => x.UserId, userId),
                    Builders<UserFavorite>.Filter.Eq(x => x.DestinationId, destinationId)
                );

                var existing = await favoriteCollection.FindAsync(filter);
                if (existing.Any())
                    return false; // Zaten favori listesinde

                var favorite = new UserFavorite
                {
                    UserId = userId,
                    DestinationId = destinationId,
                    CityCountry = cityCountry,
                    ImageUrl = imageUrl,
                    Price = price,
                    CreatedDate = DateTime.UtcNow
                };

                await favoriteCollection.InsertOneAsync(favorite);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Favori eklenirken hata oluştu: {ex.Message}");
            }
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, string favoriteId)
        {
            try
            {
                var favoriteCollection = _mongoDatabase.GetCollection<UserFavorite>("UserFavorites");

                var filter = Builders<UserFavorite>.Filter.And(
                    Builders<UserFavorite>.Filter.Eq(x => x.Id, favoriteId),
                    Builders<UserFavorite>.Filter.Eq(x => x.UserId, userId)
                );

                var result = await favoriteCollection.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Favori kaldırılırken hata oluştu: {ex.Message}");
            }
        }

        public async Task<List<UserFavoriteDto>> GetFavoritesAsync(string userId)
        {
            try
            {
                var favoriteCollection = _mongoDatabase.GetCollection<UserFavorite>("UserFavorites");

                var filter = Builders<UserFavorite>.Filter.Eq(x => x.UserId, userId);
                var sort = Builders<UserFavorite>.Sort.Descending(x => x.CreatedDate);

                var favorites = await favoriteCollection
                    .Find(filter)
                    .Sort(sort)
                    .ToListAsync();

                return _mapper.Map<List<UserFavoriteDto>>(favorites);
            }
            catch (Exception ex)
            {
                throw new Exception($"Favoriler yüklenirken hata oluştu: {ex.Message}");
            }
        }
    }
}