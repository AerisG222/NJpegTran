using System.IO;


namespace NJpegTran
{
    public class Result
    {
        public Stream OutputStream { get; set; }
        public bool Success { get; set; }
        public int ExitCode { get; set; }
    }
}
