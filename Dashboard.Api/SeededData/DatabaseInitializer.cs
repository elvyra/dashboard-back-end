using Dashboard.Data;
using Dashboard.Hash;
using Dashboard.Models;
using Dashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dashboard.Api.SeededData
{
    public class DatabaseInitializer
    {
        public static bool Initialize(DashboardDbContext context, IOptions<MainUserData> options)
        {
            context.Database.EnsureCreated();
            if (string.IsNullOrWhiteSpace(options.Value.Email) ||
                string.IsNullOrWhiteSpace(options.Value.Password))
                return false;

            var email = options.Value.Email;
            var password = options.Value.Password;
            var name = options.Value.Name ?? "";
            var surname = options.Value.Surname ?? "";

            if (!IsValidEmail(email))
                return false;
                       
            var userInDb = context.Users.Where(u => u.Email == email).FirstOrDefault();

            if (userInDb == null)
            {
                var user = new User()
                    {
                        Email = email,
                        Name = name,
                        Surname = surname,
                        Password = new HashService().Hash(password),
                        IsActive = true,
                        isPermanent = true,
                        Claims = new string[] { ClaimType.isAdmin.ToString() }
                    };
                try
                {
                    context.Add(user);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            userInDb.Password = new HashService().Hash(password);
            userInDb.Name = name;
            userInDb.Surname = surname;
            try
            {
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if string is a valid email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
