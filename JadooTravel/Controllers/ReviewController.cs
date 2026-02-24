using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.ReviewDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JadooTravel.Entity.Entities;

namespace JadooTravel.UI.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;

        public ReviewController(
            IReviewService reviewService,
            UserManager<AppUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                var reviews = await _reviewService.GetUserReviewsAsync(user.Id);
                return View(reviews);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(new List<UserReviewDto>());
            }
        }

        [HttpGet]
        public IActionResult CreateReview(string destinationId)
        {
            ViewBag.DestinationId = destinationId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(CreateReviewDto createReviewDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(createReviewDto);

            try
            {
                await _reviewService.CreateAsync(createReviewDto, user.Id);
                TempData["success"] = "Yorumunuz başarıyla oluşturuldu. Admin onayı beklemektedir.";
                return RedirectToAction("MyReviews");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(createReviewDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditReview(string reviewId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                var review = await _reviewService.GetByIdAsync(reviewId);
                if (review.UserId != user.Id)
                    return Forbid();

                var updateDto = new UpdateReviewDto
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Title = review.Title,
                    Comment = review.Comment,
                    VisitedDays = review.VisitedDays,
                    Tags = review.Tags
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("MyReviews");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(UpdateReviewDto updateReviewDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(updateReviewDto);

            try
            {
                var review = await _reviewService.GetByIdAsync(updateReviewDto.Id);
                if (review.UserId != user.Id)
                    return Forbid();

                await _reviewService.UpdateAsync(updateReviewDto);
                TempData["success"] = "Yorumunuz güncellenmiştir. Tekrar onaya gönderildi.";
                return RedirectToAction("MyReviews");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(updateReviewDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Lütfen giriş yapınız" });

            try
            {
                var review = await _reviewService.GetByIdAsync(reviewId);
                if (review.UserId != user.Id)
                    return Json(new { success = false, message = "Bu yorumu silemezsiniz" });

                await _reviewService.DeleteAsync(reviewId);
                return Json(new { success = true, message = "Yorum silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkHelpful(string reviewId)
        {
            try
            {
                await _reviewService.MarkHelpfulAsync(reviewId);
                return Json(new { success = true, message = "Teşekkürler!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Destination detail sayfasında gösterilecek yorumlar
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDestinationReviews(string destinationId)
        {
            try
            {
                var reviews = await _reviewService.GetApprovedReviewsAsync(destinationId);
                return PartialView("_ReviewsList", reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDestinationReviewSummary(string destinationId)
        {
            try
            {
                var summary = await _reviewService.GetDestinationReviewSummaryAsync(destinationId);
                return Json(summary);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertInline(string? reviewId, string destinationId, int rating, string title, string comment, int visitedDays)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Lütfen giriş yapınız" });

            if (string.IsNullOrWhiteSpace(destinationId) || rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(comment))
            {
                return Json(new { success = false, message = "Lütfen tüm alanları doğru doldurunuz" });
            }

            try
            {
                if (string.IsNullOrWhiteSpace(reviewId))
                {
                    await _reviewService.CreateAsync(new CreateReviewDto
                    {
                        DestinationId = destinationId,
                        Rating = rating,
                        Title = title,
                        Comment = comment,
                        VisitedDays = visitedDays,
                        Tags = new List<string>()
                    }, user.Id);

                    return Json(new { success = true, message = "Yorum eklendi. Onay sonrası görünür olacaktır." });
                }

                var review = await _reviewService.GetByIdAsync(reviewId);
                if (review == null || review.UserId != user.Id)
                    return Json(new { success = false, message = "Bu yorumu güncelleyemezsiniz" });

                await _reviewService.UpdateAsync(new UpdateReviewDto
                {
                    Id = reviewId,
                    Rating = rating,
                    Title = title,
                    Comment = comment,
                    VisitedDays = visitedDays,
                    Tags = review.Tags ?? new List<string>()
                });

                return Json(new { success = true, message = "Yorum güncellendi. Tekrar onaya gönderildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}