using System.ComponentModel.DataAnnotations.Schema;

namespace DbUpgrade.EF
{
    [Table("VersionDatabase")]
    public class DbVersion
    {
        public int Id { get; set; }
        public string DatabasePrefix { get; set; }
        public string Module { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
    }
}
