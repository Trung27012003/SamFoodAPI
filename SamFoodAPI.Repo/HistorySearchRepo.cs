using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class HistorySearchRepo : GenericRepo<HistorySearch>
    {
        public HistorySearchRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
