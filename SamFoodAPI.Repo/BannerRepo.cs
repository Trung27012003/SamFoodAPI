using Microsoft.EntityFrameworkCore;
using SamFoodAPI.Model.Context;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SamFoodAPI.Repo;

public class BannerRepo : GenericRepo<Banner>
{
    public BannerRepo(CurrentUser currentUser) : base(currentUser)
    {
    }

}