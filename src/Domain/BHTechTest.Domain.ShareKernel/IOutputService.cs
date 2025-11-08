using BHTechTest.Domain.ShareKernel.Results;

namespace BHTechTest.Domain.ShareKernel
{
    public interface IOutputService
    {
        void WriteLine(string message);
        void Write(Result result);
        void Write<T>(Result<T> result);
    }
}
