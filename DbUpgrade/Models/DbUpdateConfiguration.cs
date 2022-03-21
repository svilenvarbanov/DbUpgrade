namespace DbUpgrade.Models
{
    public class DbUpConfiguration
    {
        public string Name { get; set; }
        public string ScriptsRootPath { get; set; }
        public string Module { get; set; }
        public string DatabasePrefix { get; set; }
        public string ConnectionString { get; set; }
        public bool CheckoutLastRepoVersion { get; set; }
    }
}
