using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType(TypeName = "Shell")]
    public class ImportShellDto
    {
        [XmlElement]
        [Required]
        [Range(2.0, 1680.0)]
        public double ShellWeight { get; set; }

        [XmlElement]
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Caliber { get; set; }
    }
}
