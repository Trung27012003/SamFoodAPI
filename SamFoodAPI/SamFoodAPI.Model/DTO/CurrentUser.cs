using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Model.DTO
{
    public class CurrentUser
    {
        public int ID { get; set; }
        public string LoginName { get; set; } = string.Empty;
    }
}
