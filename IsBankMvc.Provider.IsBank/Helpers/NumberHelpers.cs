using System.Globalization;

namespace IsBankMvc.Provider.IsBank.Helpers
{
    public static class NumberHelpers
    {
        private static readonly CultureInfo _culture = new("en-US");

        public static decimal ParseMoney(string input)
        {
            return decimal.Parse(input, _culture);
        }

        public static string ToString(decimal input)
        {
            return input.ToString(_culture);
        }

        public static int ParseNumber(string input)
        {
            return int.Parse(input, _culture);
        }

        public static string ToString(int input)
        {
            return input.ToString(_culture);
        }
    }
}
