using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType(TypeName = "Cast")]
    public class CastDto
    {
        [XmlElement]
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string FullName { get; set; }

        [XmlElement]
        [Required]
        public bool IsMainCharacter { get; set; }

        [XmlElement]
        [Required]
        [RegularExpression(@"[+]44-[0-9]{2}-[0-9]{3}-[0-9]{4}")]
        public string PhoneNumber { get; set; }

        [XmlElement]
        [Required]
        public int PlayId { get; set; }
    }
}
