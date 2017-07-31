using System;
namespace Matrix.Types
{
    public interface IServer
    {
        //void LogWrite(LogType lt,string message);
        Tag ReadTag(string tagName);
        Tag[] ReadTags(string[] tagNames);
        Tag[] ReadTags(ref DateTime dt, string[] tagnames);
        void SetTag(Tag tag);
        Tag GetTag(string tagName);
        void WriteTag(Tag tag);
        void WriteTags(Tag[] tags);
    }
}
