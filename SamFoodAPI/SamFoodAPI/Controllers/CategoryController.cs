using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;
using System.Threading.Tasks;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
      
        private readonly CategoryRepo _categoryRepo;
        public CategoryController(CategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var categorys = _categoryRepo.GetAll();
                return Ok(ApiResponseFactory.Success(categorys));
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
                var category = await _categoryRepo.GetByIDAsync(id);
                return Ok(ApiResponseFactory.Success(category));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] Category category)
        {
            try
            {
                int result = 0;
                if (category.ID <= 0) result = await _categoryRepo.CreateAsync(category);
                else result = await _categoryRepo.UpdateAsync(category);

                if(result > 0) return Ok(ApiResponseFactory.Success(category,"Cập nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!",category));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
