using BHTechTest.Domain.ShareKernel;
using BHTechTest.Domain.ShareKernel.Results;

namespace BHTechTest.App.WebApi
{
    public class WebApiOutputService : IOutputService
    {
        public void Write(Result result)
        {
            throw new System.NotImplementedException();
        }

        public void Write<T>(Result<T> result)
        {
            throw new System.NotImplementedException();
        }

        public void WriteLine(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
