using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Medallion.Shell;


namespace NJpegTran
{
    public class JpegTran
    {
        public Options Options { get; private set; }


        public JpegTran(Options opts)
        {
            Options = opts ?? throw new ArgumentNullException(nameof(opts));
        }


        public async Task<Result> RunAsync(string srcPath)
        {
            ValidateSourceFile(srcPath);

            var args = Options.GetArguments(srcPath, null);

            var result = await RunProcessAsync(args, null, srcPath, null);

            return result;
        }


        public async Task<Result> RunAsync(string srcPath, string dstPath)
        {
            ValidateSourceFile(srcPath);

            var args = Options.GetArguments(srcPath, dstPath);

            var result = await RunProcessAsync(args, null, srcPath, dstPath);

            return result;
        }


        public async Task<Result> RunAsync(Stream inStream)
        {
            ValidateStream(inStream);

            var args = Options.GetArguments(null, null);

            return await RunProcessAsync(args, inStream, null, null);
        }


        public async Task<Result> RunAsync(Stream inStream, string dstPath)
        {
            ValidateStream(inStream);

            var args = Options.GetArguments(null, dstPath);

            return await RunProcessAsync(args, inStream, null, dstPath);
        }


        async Task<Result> RunProcessAsync(string[] args, Stream srcStream, string srcPath, string dstPath)
        {
            Command cmd = null;
            MemoryStream ms = null;

            try
            {
                if(srcStream == null)
                {
                    if(dstPath == null)
                    {
                        ms = new MemoryStream();
                        cmd = Command.Run(Options.JpegTranPath, args) > ms;
                    }
                    else
                    {
                        cmd = Command.Run(Options.JpegTranPath, args);
                    }
                }
                else
                {
                    if(dstPath == null)
                    {
                        ms = new MemoryStream();
                        cmd = Command.Run(Options.JpegTranPath, args) < srcStream > ms;
                    }
                    else
                    {
                        cmd = Command.Run(Options.JpegTranPath, args) < srcStream;
                    }
                }

                await cmd.Task;

                if(!cmd.Result.Success)
                {
                    return new Result {
                        Success = false,
                        ExitCode = cmd.Result.ExitCode
                    };
                }

                if(ms != null)
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    return new Result {
                        Success = true,
                        ExitCode = cmd.Result.ExitCode,
                        OutputStream = ms
                    };
                }
                else
                {
                    return new Result {
                        Success = true,
                        ExitCode = cmd.Result.ExitCode
                    };
                }
            }
            catch (Win32Exception ex)
            {
                throw new Exception("Error when trying to start the jpegtran process.  Please make sure it is installed, and its path is properly specified in the options.", ex);
            }
        }


        void ValidateSourceFile(string srcPath)
        {
            if(!File.Exists(srcPath))
            {
                throw new FileNotFoundException("Please make sure the image exists.", srcPath);
            }
        }


        void ValidateStream(Stream inStream)
        {
            if(inStream == null)
            {
                throw new ArgumentNullException(nameof(inStream));
            }
        }
    }
}
