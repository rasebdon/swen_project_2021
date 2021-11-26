namespace MTCG.Models
{
    public enum OutputFormat
    {
        Standard,
        Warning,
        Error,
        Success
    }

    public interface ILog
    {
        void Write(string msg);
        void Write(string msg, OutputFormat format);
        void WriteLine(string msg);
        void WriteLine(string msg, OutputFormat format);
    }
}
