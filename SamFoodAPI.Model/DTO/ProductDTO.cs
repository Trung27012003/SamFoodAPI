using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Model.DTO
{
    public class ProductDTO:Product
    {
        public List<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
        public List<ProductProcessingRecipe> ProductProcessingRecipes { get; set; } = new List<ProductProcessingRecipe>();
    }
}
