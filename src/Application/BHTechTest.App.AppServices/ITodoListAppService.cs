namespace BHTechTest.App.AppServices
{   
    public interface ITodoListAppService
    {
        void Add(string args);
        void Update(string args);
        void Remove(string args);
        void Progress(string args);
        void PrintItems();
    }
}
