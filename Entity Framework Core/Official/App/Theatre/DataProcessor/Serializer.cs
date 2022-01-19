namespace Theatre.DataProcessor
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            List<TheatreExportDto> theatresToExport = new List<TheatreExportDto>();

            var theatres = context.Theatres
                .Include(t => t.Tickets)
                .Where(t => t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count >= 20)
                .ToList()
                .Select(t => new
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets
                        .Where(t => t.RowNumber <= 5 && t.RowNumber >= 1)
                        .Sum(t => t.Price),
                    Tickets = t.Tickets
                        .Where(t => t.RowNumber <= 5 && t.RowNumber >= 1)
                        .OrderByDescending(t => t.Price)
                        .Select(t => new
                        {
                            Price = decimal.Parse(t.Price.ToString("0.00")),
                            RowNumber = t.RowNumber
                        })
                        .ToList()
                        
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToList();
        
            return JsonConvert.SerializeObject(theatres, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Plays");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlayExportDto[]), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter sw = new StringWriter(sb);

            var plays = context.Plays
                .Where(p => p.Rating <= rating)
                .ToArray()
                .Select(p => new PlayExportDto
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString(),
                    Rating = p.Rating == 0 ? "Premier" : $"{p.Rating}",
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts
                        .Where(c => c.IsMainCharacter)
                        .Select(c => new PlayActorExportDto
                        {
                            FullName = c.FullName,
                            MainCharacter = $"Plays main character in '{p.Title}'."
                        })
                        .OrderByDescending(c => c.FullName)
                        .ToArray()

                })
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre.ToString())
                .ToArray();

            xmlSerializer.Serialize(sw, plays, namespaces);
            return sb.ToString().TrimEnd();
        }
    }
}
