namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Plays");
            XmlSerializer serializer = new XmlSerializer(typeof(PlayDto[]), root);
            PlayDto[] plays;
            List<Play> dbPlays = new List<Play>();

            using (StringReader sr = new StringReader(xmlString))
            {
                plays = (PlayDto[])serializer.Deserialize(sr);
            }

            foreach (var play in plays)
            {
                if (!IsValid(play))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Play dbPlay = new Play()
                {
                    Title = play.Title,
                    Rating = play.Rating,
                    Description = play.Description,
                    Screenwriter = play.Screenwriter
                };

                if (play.Title.Length < 4 || play.Title.Length > 50)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                if (!Enum.TryParse(typeof(Genre), play.Genre, out object playGenre))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                dbPlay.Genre = (Genre)playGenre;

                if (!TimeSpan.TryParseExact(play.Duration, "c", CultureInfo.InvariantCulture, TimeSpanStyles.None, out TimeSpan playDuration))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (playDuration.Hours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                dbPlay.Duration = playDuration;
                dbPlays.Add(dbPlay);
                sb.AppendLine(string.Format(SuccessfulImportPlay, dbPlay.Title, dbPlay.Genre, dbPlay.Rating));
            }

            context.Plays.AddRange(dbPlays);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Casts");
            XmlSerializer serializer = new XmlSerializer(typeof(CastDto[]), root);
            CastDto[] casts;
            List<Cast> dbCasts = new List<Cast>();

            using (StringReader sr = new StringReader(xmlString))
            {
                casts = (CastDto[])serializer.Deserialize(sr);
            }

            foreach (var cast in casts)
            {
                if (!IsValid(cast))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Cast dbCast = new Cast()
                {
                    FullName = cast.FullName,
                    IsMainCharacter = cast.IsMainCharacter,
                    PhoneNumber = cast.PhoneNumber,
                    PlayId = cast.PlayId
                };

                dbCasts.Add(dbCast);
                string status = "lesser";
                if (dbCast.IsMainCharacter)
                {
                    status = "main";
                }
                sb.AppendLine(string.Format(SuccessfulImportActor, dbCast.FullName, status));
            }

            context.Casts.AddRange(dbCasts);
            context.SaveChanges();

            return sb.ToString().Trim();
        }


        //TODO
        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var theatres = JsonConvert.DeserializeObject<TheatreDto[]>(jsonString);
            List<Theatre> theatresList = new List<Theatre>();
            
            foreach (var theatre in theatres)
            {
                if (!IsValid(theatre))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre dbTheatre = new Theatre()
                {
                    Name = theatre.Name,
                    NumberOfHalls = theatre.NumberOfHalls,
                    Director = theatre.Director,
                    Tickets = new List<Ticket>()
                };

                foreach (var ticket in theatre.Tickets)
                {
                    if (!IsValid(ticket))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket dbTicket = new Ticket()
                    {
                        Price = ticket.Price,
                        RowNumber = ticket.RowNumber,
                        PlayId = ticket.PlayId,
                        TheatreId = dbTheatre.Id
                    };

                    dbTheatre.Tickets.Add(dbTicket);
                }

                theatresList.Add(dbTheatre);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, theatre.Name, dbTheatre.Tickets.Count));
            }

            context.Theatres.AddRange(theatresList);
            context.SaveChanges();

            return sb.ToString().Trim();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
