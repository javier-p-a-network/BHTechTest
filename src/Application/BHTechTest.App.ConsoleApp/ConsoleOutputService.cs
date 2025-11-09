using BHTechTest.Domain.ShareKernel;
using BHTechTest.Domain.ShareKernel.Results;

namespace BHTechTest.App.ConsoleApp
{
    public class ConsoleOutputService : IOutputService
    {
        public void WriteLine(string message) =>
            Console.WriteLine(message);

        public void Write(Result result)
        {
            foreach (var resultMessage in result.Messages)
                WriteLine($"[{resultMessage.Type}]: {resultMessage.Text}");
        }

        public void Write<T>(Result<T> result)
        {
            foreach (var resultMessage in result.Messages)
                WriteLine($"[{resultMessage.Type}]: {resultMessage.Text}");
        }
    }
}
