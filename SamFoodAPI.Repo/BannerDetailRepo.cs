using SamFoodAPI.Model.DTO;

namespace SamFoodAPI.Repo;

public class BannerDetailRepo : GenericRepo<Model.Entities.BannerDetail>
{
    public BannerDetailRepo(CurrentUser currentUser) : base(currentUser)
    {
    }
}