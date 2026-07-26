using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorySearchController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HistorySearchRepo _historySearchRepo;

        public HistorySearchController(HistorySearchRepo historySearchRepo, IConfiguration configuration)
        {
            _historySearchRepo = historySearchRepo;
            _configuration = configuration;
        }

        [HttpGet()]
        public IActionResult GetAll(string? keyword)
        {
            try
            {
                keyword = keyword ?? "";
                var historySearchs = _historySearchRepo.GetAll();
                return Ok(ApiResponseFactory.Success(historySearchs));
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
                var historySearch = await _historySearchRepo.GetByIDAsync(id);
                return Ok(ApiResponseFactory.Success(historySearch));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] HistorySearch historySearch)
        {
            try
            {
                int result = 0;
                if (historySearch.ID <= 0) result = await _historySearchRepo.CreateAsync(historySearch);
                else result = await _historySearchRepo.UpdateAsync(historySearch);

                if (result > 0) return Ok(ApiResponseFactory.Success(historySearch, "Cập nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
