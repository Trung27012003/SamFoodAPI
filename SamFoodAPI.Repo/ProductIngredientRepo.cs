using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class ProductIngredientRepo : GenericRepo<ProductIngredient>
    {
        public ProductIngredientRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<int> Create(ProductDTO product)
        {
            try
            {
                int record = 0;
                var data = GetAll(x => x.ProductID == product.ID && x.IsDeleted != true);

                //Xóa hết ds chi tiết
                foreach (var item in data)
                {
                    item.IsDeleted = true;
                    await UpdateAsync(item);
                }
                data.Clear();

                if (product.IsDeleted != true)
                {
                    product.ProductIngredients.ForEach(x => x.ProductID = product.ID);
                    record = await CreateRangeAsync(product.ProductIngredients);
                }
                else record = 1;

                return record;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
