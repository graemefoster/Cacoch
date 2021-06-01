namespace Cacoch.Core.Manifest
{
    public class WebApp: IResource
    {
        public WebApp(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}