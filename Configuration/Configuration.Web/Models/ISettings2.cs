namespace Configuration.Web.Models
{
    public interface ISettings2
    {
        string Address { get; set; }
    }

    class Settings2 : ISettings2
    {
        public string Address { get; set; }
    }
}