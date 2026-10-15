using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;

namespace ClassLibTeam12.Business.Validation
{
    public static class ValidationRules
    {
        /// <summary>
        /// Valideert een telefoonnummer (alleen cijfers of beginnend met een +).
        /// </summary>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Regex voor telefoonnummers met optionele "+" en lengte tussen 10 en 15 cijfers
            string pattern = @"^\+?[0-9]{10,15}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        /// <summary>
        /// Valideert een e-mailadres (moet een @ en een . bevatten).
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// Valideert een naam (alleen letters, geen cijfers of speciale tekens).
        /// </summary>
        public static bool IsValidName(string name)
        {
            string pattern = @"^[a-zA-Z]+$"; // Alleen letters (hoofdletters en kleine letters)
            return Regex.IsMatch(name, pattern);
        }

        /// <summary>
        /// Maskeert een e-mailadres (bijv. "t***@mail.com").
        /// </summary>
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return email;

            int atIndex = email.IndexOf('@');
            if (atIndex <= 0) // Correctie: Moet groter zijn dan 0
                return email;

            string firstLetter = email.Substring(0, 1);
            string maskedUserPart = new string('*', 3); // Gebruik altijd 3 sterren
            string domainPart = email.Substring(atIndex);
            return firstLetter + maskedUserPart + domainPart;
        }

        /// <summary>
        /// Maskeert een telefoonnummer (alleen de laatste twee cijfers blijven zichtbaar).
        /// </summary>
        public static string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return phone;

            if (phone.Length < 2) return phone;

            // Controleer of het nummer begint met een '+'
            if (phone.StartsWith("+"))
            {
                // Als het nummer begint met '+', neem de eerste 3 tekens
                return phone.Substring(0, 3) + "*******" + phone.Substring(phone.Length - 2);
            }
            else
            {
                // Anders, neem de eerste 2 tekens
                return phone.Substring(0, 2) + "*******" + phone.Substring(phone.Length - 2);
            }
        }

        /// <summary>
        /// Valideert een wachtwoord (moet minimaal 6 tekens bevatten en een mix van cijfers en letters).
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            // Wachtwoord moet minimaal 6 tekens lang zijn, een letter en een cijfer bevatten
            string pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$";
            return Regex.IsMatch(password, pattern);
        }


    }
}