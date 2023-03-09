using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NJpegTran;

public class JpegTran
{
    readonly Options _options;

    public JpegTran(Options opts)
    {
        _options = opts ?? throw new ArgumentNullException(nameof(opts));
    }

    public Task<Result> RunAsync(string srcPath, Stream dstStream)
    {
        ValidateSourceFile(srcPath);
        ValidateDestinationStream(dstStream);

        var args = _options.GetArguments(srcPath, null);

        return RunProcessAsync(args, null, dstStream);
    }

    public Task<Result> RunAsync(string srcPath, string dstPath)
    {
        ValidateSourceFile(srcPath);

        var args = _options.GetArguments(srcPath, dstPath);

        return RunProcessAsync(args, null, null);
    }

    public Task<Result> RunAsync(Stream inStream, Stream dstStream)
    {
        ValidateSourceStream(inStream);
        ValidateDestinationStream(dstStream);

        var args = _options.GetArguments(null, null);

        return RunProcessAsync(args, inStream, dstStream);
    }

    public Task<Result> RunAsync(Stream inStream, string dstPath)
    {
        ValidateSourceStream(inStream);

        var args = _options.GetArguments(null, dstPath);

        return RunProcessAsync(args, inStream, null);
    }

    async Task<Result> RunProcessAsync(string[] args, Stream srcStream, Stream dstStream)
    {
        using var process = new Process();

        process.StartInfo.FileName = _options.JpegTranPath;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.StandardInputEncoding = Console.InputEncoding;
        process.StartInfo.StandardErrorEncoding = Console.OutputEncoding;
        process.StartInfo.StandardOutputEncoding = Console.OutputEncoding;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;

        foreach(var arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);
        }

        try
        {
            var stdErr = new StringBuilder();
            process.ErrorDataReceived += (sender, e) => stdErr.Append(e.Data);

            process.Start();

            if(srcStream != null)
            {
                await srcStream.CopyToAsync(process.StandardInput.BaseStream).ConfigureAwait(false);
                await process.StandardInput.FlushAsync().ConfigureAwait(false);
                process.StandardInput.Close();
            }

            process.BeginErrorReadLine();

            if(dstStream != null)
            {
                await process.StandardOutput.BaseStream.CopyToAsync(dstStream).ConfigureAwait(false);
                await process.StandardOutput.BaseStream.FlushAsync().ConfigureAwait(false);
                process.StandardOutput.Close();
            }

            await process.WaitForExitAsync().ConfigureAwait(false);

            return new Result {
                Success = process.ExitCode == 0,
                ExitCode = process.ExitCode,
                StdError = stdErr.ToString()
            };
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

    void ValidateSourceStream(Stream sourceStream)
    {
        if(sourceStream == null)
        {
            throw new ArgumentNullException(nameof(sourceStream));
        }

        if(!sourceStream.CanRead)
        {
            throw new InvalidOperationException("Unable to read from source stream!");
        }
    }

    void ValidateDestinationStream(Stream dstStream)
    {
        if(dstStream == null)
        {
            throw new ArgumentNullException(nameof(dstStream));
        }

        if(!dstStream.CanWrite)
        {
            throw new InvalidOperationException("Unable to write to destination stream!");
        }
    }
}
