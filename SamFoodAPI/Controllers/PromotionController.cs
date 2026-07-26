using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PromotionRepo _promotionRepo;

        public PromotionController(PromotionRepo promotionRepo, IConfiguration configuration)
        {
            _promotionRepo = promotionRepo;
            _configuration = configuration;
        }

        [HttpGet()]
        public IActionResult GetAll(string? keyword)
        {
            try
            {
                keyword = keyword ?? "";
                var promotions = _promotionRepo.GetAll();
                return Ok(ApiResponseFactory.Success(promotions));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var category = await _promotionRepo.GetByIDAsync(id);
                return Ok(ApiResponseFactory.Success(category));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] Promotion promotion)
        {
            try
            {
                int result = 0;
                if (promotion.ID <= 0) result = await _promotionRepo.CreateAsync(promotion);
                else result = await _promotionRepo.UpdateAsync(promotion);

                if (result > 0) return Ok(ApiResponseFactory.Success(promotion, "Cập nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!", promotion));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                //_currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                //var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                //_currentUser = ObjectMapper.GetCurrentUser(claims);

                var form = await Request.ReadFormAsync();
                var promotionID = Convert.ToInt32(form["PromotionID"]);

                var files = Request.Form.Files;

                var pathServer = _configuration.GetValue<string>("ImagePath");
                if (string.IsNullOrWhiteSpace(pathServer))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn!"));
                }

                var promotion = await _promotionRepo.GetByIDAsync(promotionID);

                string pathPattern = $@"Promotion/{promotion.PromotionCode}";
                string pathUpload = Path.Combine(pathServer, pathPattern);

                foreach (var file in files)
                {
                    var result = await FileHelper.UploadFileAsync(file, pathUpload);

                    if (result.status == 1)
                    {
                        promotion.BannerImg = Convert.ToString(result.data);
                        await _promotionRepo.UpdateAsync(promotion);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
