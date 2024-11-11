using System.Text.RegularExpressions;

namespace IsBankMvc.Abstraction.Extensions
{
    public static class StringExtensions
    {
        public static string ObfuscateCreditCardNumber(this string cardNumber)
        {
            var last4digit = cardNumber.Substring(cardNumber.Length - 4);
            return $"**** **** **** {last4digit}";
        }

        public static Tuple<string, string> ExtractPassportInfo(this string passportNumber)
        {
            var firstDigitIndex = Regex.Match(passportNumber, @"\d").Index;

            // Ensure that a digit is found and the serial part is at least one character long.
            if (firstDigitIndex <= 0 || firstDigitIndex >= passportNumber.Length)
                throw new ArgumentException("Invalid passport number format.");

            var serial = passportNumber[..firstDigitIndex];
            var digits = passportNumber[firstDigitIndex..];
            return new Tuple<string, string>(serial, digits);
        }

        public static Tuple<string, string> ExtractNameSurname(this string fullName)
        {
            var parts = fullName.Trim().Split(' ').ToList();

            var name = parts[0];
            parts.RemoveAt(0);

            var surname = string.Join(' ', parts);
            return new Tuple<string, string>(name, surname);
        }
    }
}
