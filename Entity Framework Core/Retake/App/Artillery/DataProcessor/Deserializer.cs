namespace Artillery.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage =
                "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Countries");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCountryDto[]), xmlRoot);
            ImportCountryDto[] countries;
            List<Country> dbCountries = new List<Country>();

            using (StringReader sr = new StringReader(xmlString))
            {
                countries = (ImportCountryDto[])serializer.Deserialize(sr);
            }

            foreach (var dto in countries)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Country dbCountry = new Country()
                {
                    CountryName = dto.CountryName,
                    ArmySize = dto.ArmySize
                };

                dbCountries.Add(dbCountry);
                sb.AppendLine(String.Format(SuccessfulImportCountry, dbCountry.CountryName, dbCountry.ArmySize));
            }

            context.Countries.AddRange(dbCountries);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Manufacturers");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportManufacturerDto[]), xmlRoot);
            ImportManufacturerDto[] manufacturers;
            List<Manufacturer> dbManufacturers = new List<Manufacturer>();

            using (StringReader sr = new StringReader(xmlString))
            {
                manufacturers = (ImportManufacturerDto[])serializer.Deserialize(sr);
            }

            foreach (var dto in manufacturers)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var manufacturerExists = dbManufacturers.Any(x => x.ManufacturerName == dto.ManufacturerName);
                if (manufacturerExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                string[] founded = dto.Founded.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToArray();

                if (founded.Length < 2)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Manufacturer dbManufacturer = new Manufacturer()
                {
                    ManufacturerName = dto.ManufacturerName,
                    Founded = dto.Founded
                };
                dbManufacturers.Add(dbManufacturer);
                
                sb.AppendLine($"Successfully import manufacturer {dbManufacturer.ManufacturerName} founded in {founded[founded.Length - 2]}, {founded[founded.Length - 1]}.");
            }

            context.Manufacturers.AddRange(dbManufacturers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Shells");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportShellDto[]), xmlRoot);
            ImportShellDto[] shells;
            List<Shell> dbShells = new List<Shell>();

            using (StringReader sr = new StringReader(xmlString))
            {
                shells = (ImportShellDto[])serializer.Deserialize(sr);
            }

            foreach (var dto in shells)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Shell dbShell = new Shell()
                {
                    ShellWeight = dto.ShellWeight,
                    Caliber = dto.Caliber
                };

                dbShells.Add(dbShell);
                sb.AppendLine(String.Format(SuccessfulImportShell, dbShell.Caliber, dbShell.ShellWeight));
            }

            context.AddRange(dbShells);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ImportGunDto[] gunDtos = JsonConvert.DeserializeObject<ImportGunDto[]>(jsonString);

            List<Gun> guns = new List<Gun>();

            foreach (var dto in gunDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.TryParse(dto.GunType, out GunType gunType))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                
                Gun gun = new Gun()
                {
                    ManufacturerId = dto.ManufacturerId,
                    GunWeight = dto.GunWeight,
                    BarrelLength = dto.BarrelLength,
                    NumberBuild = dto.NumberBuild,
                    Range = dto.Range,
                    GunType = gunType,
                    ShellId = dto.ShellId,
                };

                HashSet<CountryGun> countryGuns = new HashSet<CountryGun>();
                foreach (var cgDto in dto.Countries.Distinct())
                {
                    Country country = context.Countries.FirstOrDefault(c => c.Id == cgDto.Id);

                    CountryGun countryGun = new CountryGun()
                    {
                        Country = country,
                        Gun = gun
                    };
                    countryGuns.Add(countryGun);
                }
                gun.CountriesGuns = countryGuns;

                guns.Add(gun);

                sb.AppendLine(String.Format(SuccessfulImportGun, gun.GunType.ToString(), gun.GunWeight, gun.BarrelLength));
            }

            context.AddRange(guns);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
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
