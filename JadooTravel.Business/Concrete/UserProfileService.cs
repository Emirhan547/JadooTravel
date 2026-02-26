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

        private readonly IBookingDal _bookingDal;

        public UserProfileService(
            UserManager<AppUser> userManager,
             IBookingDal bookingDal
            )
        {
            _userManager = userManager;
          
            _bookingDal = bookingDal;
      
        }

        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var bookingCount = await _bookingDal.CountByUserIdAsync(userId);
           

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
                TotalBookings = (int)bookingCount
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

       
    }


}