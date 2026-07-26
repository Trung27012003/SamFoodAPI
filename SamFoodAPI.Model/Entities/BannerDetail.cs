using System;
using System.Text.Json.Serialization;

namespace SamFoodAPI.Model.Entities;

public partial class BannerDetail
{
    public int ID { get; set; }
    public int BannerID { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
    public string? LinkURL { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public virtual Banner? Banner { get; set; }
}
