using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.ReviewDtos;
using JadooTravel.UI.Areas.Admin.Models;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;
        private readonly IElasticAuditLogger _auditLogger;
        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger, IElasticAuditLogger auditLogger)
        {
            _reviewService = reviewService;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = await _reviewService.GetReviewStatsAsync();
                var reviews = await _reviewService.GetAllReviewsAsync();
                await _auditLogger.LogAsync("admin.review.list", "admin", User.Identity?.Name, "list", "review", null, "success", new { count = reviews.Count });

                return View(new AdminReviewListViewModel
                {
                    Stats = stats,
                    Reviews = reviews
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin review index failed. AdminUser: {AdminUser}", User.Identity?.Name);
                await _auditLogger.LogAsync("admin.review.list", "admin", User.Identity?.Name, "list", "review", null, "error", new { ex.Message });
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
                await _auditLogger.LogAsync("admin.review.delete", "admin", User.Identity?.Name, "delete", "review", reviewId, "success");
                return Json(new { success = true, message = "Yorum silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin delete review failed. AdminUser: {AdminUser}, ReviewId: {ReviewId}", User.Identity?.Name, reviewId);
                await _auditLogger.LogAsync("admin.review.delete", "admin", User.Identity?.Name, "delete", "review", reviewId, "error", new { ex.Message });
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> PendingReviews()
        {
            try
            {
                var reviews = await _reviewService.GetPendingReviewsAsync();
                await _auditLogger.LogAsync("admin.review.pending", "admin", User.Identity?.Name, "list", "review", null, "success", new { count = reviews.Count });
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin pending reviews failed. AdminUser: {AdminUser}", User.Identity?.Name);
                await _auditLogger.LogAsync("admin.review.pending", "admin", User.Identity?.Name, "list", "review", null, "error", new { ex.Message });
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
                await _auditLogger.LogAsync("admin.review.approve", "admin", User.Identity?.Name, "approve", "review", reviewId, "success");
                return Json(new { success = true, message = "Yorum onaylandı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin approve review failed. AdminUser: {AdminUser}, ReviewId: {ReviewId}", User.Identity?.Name, reviewId);
                await _auditLogger.LogAsync("admin.review.approve", "admin", User.Identity?.Name, "approve", "review", reviewId, "error", new { ex.Message });
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectReview(string reviewId, string reason)
        {
            try
            {
                await _reviewService.RejectReviewAsync(reviewId, reason);
                await _auditLogger.LogAsync("admin.review.reject", "admin", User.Identity?.Name, "reject", "review", reviewId, "success", new { reason });
                return Json(new { success = true, message = "Yorum reddedildi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin reject review failed. AdminUser: {AdminUser}, ReviewId: {ReviewId}", User.Identity?.Name, reviewId);
                await _auditLogger.LogAsync("admin.review.reject", "admin", User.Identity?.Name, "reject", "review", reviewId, "error", new { ex.Message });
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
                await _auditLogger.LogAsync("admin.review.destination", "admin", User.Identity?.Name, "list", "review", destinationId, "success", new { count = reviews.Count });

                ViewBag.Summary = summary;
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin destination reviews failed. AdminUser: {AdminUser}, DestinationId: {DestinationId}", User.Identity?.Name, destinationId);
                await _auditLogger.LogAsync("admin.review.destination", "admin", User.Identity?.Name, "list", "review", destinationId, "error", new { ex.Message });
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
                await _auditLogger.LogAsync("admin.review.details", "admin", User.Identity?.Name, "view", "review", reviewId, "success");
                return View(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin review details failed. AdminUser: {AdminUser}, ReviewId: {ReviewId}", User.Identity?.Name, reviewId);
                await _auditLogger.LogAsync("admin.review.details", "admin", User.Identity?.Name, "view", "review", reviewId, "error", new { ex.Message });
                TempData["error"] = ex.Message;
                return RedirectToAction("PendingReviews");
            }
        }
    }
}