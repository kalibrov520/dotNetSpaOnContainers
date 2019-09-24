namespace CatalogApi
{
    public class CatalogSettings : ICatalogSettings
    {
        public string PicBaseUrl { get; set; }
        public bool UseCustomizationData { get; set; }
        public string EventBusConnection { get; set; }
    }

    public interface ICatalogSettings
    {
        string PicBaseUrl { get; set; }
        bool UseCustomizationData { get; set; }
        string EventBusConnection { get; set; }
    }
}