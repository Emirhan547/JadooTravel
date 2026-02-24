using JadooTravel.Business.Abstract;
using JadooTravel.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JadooTravel.Entity.Entities;

namespace JadooTravel.UI.Controllers
{
    [AllowAnonymous]
    public class DestinationController : Controller
    {
        private readonly IDestinationService _destinationService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;

        public DestinationController(IDestinationService destinationService, IReviewService reviewService, UserManager<AppUser> userManager)
        {
            _destinationService = destinationService;
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var destinations = await _destinationService.GetAllAsync();
            return View(destinations);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var destination = await _destinationService.GetByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }

            var approvedReviews = await _reviewService.GetApprovedReviewsAsync(id);
            string? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                currentUserId = user?.Id;

                if (!string.IsNullOrEmpty(currentUserId))
                {
                    var allDestinationReviews = await _reviewService.GetDestinationReviewsAsync(id);
                    var mine = allDestinationReviews
                        .Where(x => x.UserId == currentUserId)
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();

                    if (mine != null && approvedReviews.All(x => x.Id != mine.Id))
                    {
                        approvedReviews.Insert(0, mine);
                    }
                }
            }

            var model = new DestinationDetailViewModel
            {
                Destination = destination,
                Reviews = approvedReviews.OrderByDescending(x => x.CreatedDate).ToList(),
                CurrentUserId = currentUserId
            };

            return View(model);
        }
    }
}