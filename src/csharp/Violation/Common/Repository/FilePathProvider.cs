namespace Bsvt.Common
{
    public class FilePathProvider : IPathProvider
    {
        public string GetPath()
        {
            return "db.bin";
        }
    }
}
