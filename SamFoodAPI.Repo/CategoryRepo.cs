using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class CategoryRepo : GenericRepo<Category>
    {
        public CategoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
