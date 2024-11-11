using IsBankMvc.Abstraction.Contracts;
using System.Text;

namespace IsBankMvc.Abstraction.Implementation
{
    internal class ConsoleLogger : ILoggerService
    {
        public Task Log(string message, string section)
        {
            return HandleError(ConsoleColor.DarkYellow, message, section);
        }

        public Task Info(string message, string section)
        {
            return HandleError(ConsoleColor.Blue, message, section);
        }

        public Task Debug(string message, string section)
        {
            return HandleError(ConsoleColor.Magenta, message, section);
        }

        public Task Error(string message, string section, Exception? ex = null)
        {
            var msg = BuildErrorMessage(message, ex);
            return HandleError(ConsoleColor.Red, msg, section);
        }

        private Task HandleError(ConsoleColor color, string message, string section)
        {
            Console.ResetColor();
            Console.WriteLine(GeneralConstants.LogLineSeparator);
            Console.ForegroundColor = color;
            Console.WriteLine(section);
            Console.ResetColor();
            Console.WriteLine(message);
            Console.WriteLine(GeneralConstants.LogLineSeparator);
            return Task.CompletedTask;
        }

        private string BuildErrorMessage(string msg, Exception? e)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(msg)) sb.AppendLine(msg);
            if (!string.IsNullOrEmpty(e?.Message)) sb.AppendLine(e.Message);
            if (!string.IsNullOrEmpty(e?.InnerException?.Message)) sb.AppendLine(e.InnerException.Message);
            return sb.ToString();
        }
    }
}