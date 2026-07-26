using SamFoodAPI.IRepo;
using SamFoodAPI.Model.Context;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;

namespace SamFoodAPI.Repo;

public class SiteSettingRepo : GenericRepo<SiteSetting>
{
    public SiteSettingRepo(CurrentUser currentUser) : base(currentUser)
    {
    }
}
