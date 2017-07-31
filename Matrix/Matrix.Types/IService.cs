using System;
namespace Matrix.Types
{
    public interface IService
    {
        int Id { get; set; }
        void Init(System.Xml.XmlNode xmlconf);
        string Name { get; set; }
        string Description { get; set; }
        void Run();
        void Start();
        void Stop();
        void Write(Tag tag);
        bool IsRun();
        DateTime LastRun { get; set;}
        int FailCount { get; set;}
        void Restart();
        IServer Server { set; }
        bool KeepAlive { get; set; }
        int Retry { get; set; }
        int BeforeRetry { get; set; }
        int RetryPause { get; set; }
        string ServiceStatus {get; set;}
        string ErrorMessage { get; set; }
    }
}
