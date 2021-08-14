namespace Configuration.Web.Providers.CustomProvider
{
    public interface ISettings3
    {
        string DynamicValue { get; set; }
    }

    class Settings3 : ISettings3
    {
        public string DynamicValue { get; set; }
    }
}