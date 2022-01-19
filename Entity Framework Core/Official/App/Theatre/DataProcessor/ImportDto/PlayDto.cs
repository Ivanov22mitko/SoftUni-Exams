using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType(TypeName = "Play")]
    public class PlayDto
    {
        [XmlElement]
        [Required]
        public string Title { get; set; }

        [XmlElement]
        [Required]
        public string Duration { get; set; }

        [XmlElement]
        [Required]
        public float Rating { get; set; }

        [XmlElement]
        [Required]
        public string Genre { get; set; }

        [XmlElement]
        [Required]
        public string Description { get; set; }

        [XmlElement]
        [Required]
        public string Screenwriter { get; set; }
    }
}
