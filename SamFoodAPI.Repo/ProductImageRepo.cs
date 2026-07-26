using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class ProductImageRepo : GenericRepo<ProductImage>
    {
        public ProductImageRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task UpdateIsPrimaryAsync(int productID, int imageID)
        {
            var images = GetAll(x => x.ProductID == productID);
            foreach (var img in images)
            {
                img.IsPrimary = (img.ID == imageID);
                await UpdateAsync(img);
            }
        }

        public async Task ClearAllPrimaryAsync(int productID)
        {
            var images = GetAll(x => x.ProductID == productID && x.IsPrimary == true);
            foreach (var img in images)
            {
                img.IsPrimary = false;
                await UpdateAsync(img);
            }
        }
    }
}
