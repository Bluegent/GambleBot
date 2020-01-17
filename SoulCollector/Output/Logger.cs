
namespace SoulCollector.Output
{
    public interface ILogger
    {
        void Log(string message);
        void Flush();
    }
}
