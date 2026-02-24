using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.ReviewDtos;
using JadooTravel.UI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = await _reviewService.GetReviewStatsAsync();
                var reviews = await _reviewService.GetAllReviewsAsync();

                return View(new AdminReviewListViewModel
                {
                    Stats = stats,
                    Reviews = reviews
                });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(new AdminReviewListViewModel());
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            try
            {
                await _reviewService.DeleteAsync(reviewId);
                return Json(new { success = true, message = "Yorum silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> PendingReviews()
        {
            try
            {
                var reviews = await _reviewService.GetPendingReviewsAsync();
                return View(reviews);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(new List<ResultReviewDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveReview(string reviewId, string adminNotes = "")
        {
            try
            {
                var approveDto = new ApproveReviewDto
                {
                    ReviewId = reviewId,
                    Approve = true,
                    AdminNotes = adminNotes
                };

                await _reviewService.ApproveReviewAsync(approveDto);
                return Json(new { success = true, message = "Yorum onaylandı" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectReview(string reviewId, string reason)
        {
            try
            {
                await _reviewService.RejectReviewAsync(reviewId, reason);
                return Json(new { success = true, message = "Yorum reddedildi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DestinationReviews(string destinationId)
        {
            try
            {
                var reviews = await _reviewService.GetDestinationReviewsAsync(destinationId);
                var summary = await _reviewService.GetDestinationReviewSummaryAsync(destinationId);

                ViewBag.Summary = summary;
                return View(reviews);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(new List<ResultReviewDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReviewDetails(string reviewId)
        {
            try
            {
                var review = await _reviewService.GetByIdAsync(reviewId);
                if (review == null)
                    return NotFound();

                return View(review);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("PendingReviews");
            }
        }
    }
}