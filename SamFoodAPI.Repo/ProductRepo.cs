using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class ProductRepo : GenericRepo<Product>
    {
        public ProductRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
