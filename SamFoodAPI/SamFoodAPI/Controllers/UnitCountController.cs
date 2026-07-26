using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitCountController : ControllerBase
    {
        private readonly UnitCountRepo _unitCountRepo;
        public UnitCountController(UnitCountRepo unitCountRepo)
        {
            _unitCountRepo = unitCountRepo;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var data = _unitCountRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data));
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
                var data = await _unitCountRepo.GetByIDAsync(id);
                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] UnitCount unit)
        {
            try
            {
                int result = 0;
                if (unit.ID <= 0) result = await _unitCountRepo.CreateAsync(unit);
                else result = await _unitCountRepo.UpdateAsync(unit);

                if (result > 0) return Ok(ApiResponseFactory.Success(unit, "Cập nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!", unit));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
