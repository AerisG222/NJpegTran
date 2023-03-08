using System.IO;
using Xunit;

namespace NJpegTran.Tests;

public class Tests
{
    [Fact]
    public async void CanProcessFileToFile()
    {
        var jt = GetJpegTran();
        var result = await jt.RunAsync("DSC_3982.jpg", "t1.jpg");

        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async void CanProcessFileToStream()
    {
        var jt = GetJpegTran();
        using var fs = new FileStream("t2.jpg", FileMode.Create);
        var result = await jt.RunAsync("DSC_3982.jpg", fs);

        Assert.NotNull(result);
        Assert.True(result.Success);

        await fs.FlushAsync();
        fs.Close();
    }

    [Fact]
    public async void CanProcessStreamToFile()
    {
        var jt = GetJpegTran();
        var fs = new FileStream("DSC_3982.jpg", FileMode.Open, FileAccess.Read);
        var result = await jt.RunAsync(fs, "t3.jpg");

        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async void CanProcessStreamToStream()
    {
        var jt = GetJpegTran();
        var fs = new FileStream("DSC_3982.jpg", FileMode.Open, FileAccess.Read);
        using var ofs = new FileStream("t4.jpg", FileMode.Create);

        var result = await jt.RunAsync(fs, ofs);

        Assert.NotNull(result);
        Assert.True(result.Success);

        await ofs.FlushAsync();
        ofs.Close();
    }

    [Fact]
    public void CheckArguments()
    {
        var opts = new Options {
            Optimize = true,
            Progressive = true,
            RestartMcuRows = 10,
            Arithmetic = true,
            ScansFile = "DSC_3982.jpg",
            Flip = Flip.Horizontal,
            Rotate = Rotate.Rotate180Degrees,
            Transpose = true,
            Transverse = true,
            Trim = true,
            Perfect = true,
            CropSpecification = "640x480+10+10",
            Grayscale = true,
            Copy = Copy.All,
            MaxMemoryKB = 4096
        };

        var args = string.Join(" ", opts.GetArguments("DSC_3982.jpg", "y.jpg"));

        Assert.Contains("-optimize", args);
        Assert.Contains("-progressive", args);
        Assert.Contains("-restart 10", args);
        Assert.Contains("-arithmetic", args);
        Assert.Contains("-scans", args);
        Assert.Contains("-flip horizontal", args);
        Assert.Contains("-rotate 180", args);
        Assert.Contains("-transpose", args);
        Assert.Contains("-transverse", args);
        Assert.Contains("-trim", args);
        Assert.Contains("-perfect", args);
        Assert.Contains("-crop 640x480+10+10", args);
        Assert.Contains("-grayscale", args);
        Assert.Contains("-copy all", args);
        Assert.Contains("-maxmemory 4096", args);
    }

    JpegTran GetJpegTran()
    {
        var opts = new Options {
            Optimize = true,
            Progressive = true,
            Copy = Copy.None
        };

        return new JpegTran(opts);
    }
}
