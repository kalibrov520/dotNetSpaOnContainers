using CatalogApi.Models;

namespace CatalogApi.Extensions
{
    public static class CatalogItemExtensions
    {
        public static void FillProductUrl(this CatalogItem item, string picBaseUrl)
        {
            if (item != null)
            {
                item.PictureUri = picBaseUrl.Replace("[0]", item.Id.ToString());
            }
        }
    }
}