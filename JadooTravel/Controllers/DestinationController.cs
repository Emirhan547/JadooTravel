using JadooTravel.Business.Abstract;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using JadooTravel.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Controllers
{
    [AllowAnonymous]
    public class DestinationController : Controller
    {
        private readonly IDestinationService _destinationService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IElasticAuditLogger _auditLogger;
        public DestinationController(IDestinationService destinationService, IReviewService reviewService, UserManager<AppUser> userManager, IElasticAuditLogger auditLogger)
        {
            _destinationService = destinationService;
            _reviewService = reviewService;
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var destinations = await _destinationService.GetAllAsync();
            await _auditLogger.LogAsync(
               "destination.list",
               User.Identity?.IsAuthenticated == true ? "user" : "anonymous",
               User.Identity?.IsAuthenticated == true ? _userManager.GetUserId(User) : null,
               "list",
               "destination",
               null,
               "success",
               new { count = destinations.Count });
            return View(destinations);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                await _auditLogger.LogAsync("destination.detail", "anonymous", null, "view", "destination", null, "failed", new { reason = "invalid_id" });
                return NotFound();
            }

            var destination = await _destinationService.GetByIdAsync(id);
            if (destination == null)
            {
                await _auditLogger.LogAsync("destination.detail", "anonymous", null, "view", "destination", id, "failed", new { reason = "not_found" });
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
                    
                        approvedReviews.Insert(0, mine);
                    
                }
            }

            var model = new DestinationDetailViewModel
            {
                Destination = destination,
                Reviews = approvedReviews.OrderByDescending(x => x.CreatedDate).ToList(),
                CurrentUserId = currentUserId

            };
            await _auditLogger.LogAsync(
                "destination.detail",
                string.IsNullOrEmpty(currentUserId) ? "anonymous" : "user",
                currentUserId,
                "view",
                "destination",
                id,
                "success",
                new { reviewCount = model.Reviews.Count });
            return View(model);
        }
    }
}