using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NJpegTran;

public class Options
{
    public string JpegTranPath { get; set; } = "jpegtran";
    public bool Optimize { get; set; }
    public bool Progressive { get; set; }
    public int? RestartMcuRows { get; set; }
    public int? RestartMcuBlocks { get; set; }
    public bool Arithmetic { get; set; }
    public string ScansFile { get; set; }
    public Flip Flip { get; set; }
    public Rotate Rotate { get; set; }
    public bool Transpose { get; set; }
    public bool Transverse { get; set; }
    public bool Trim { get; set; }
    public bool Perfect { get; set; }
    public string CropSpecification { get; set; }
    public bool Grayscale { get; set; }
    public Copy Copy { get; set; }
    public int? MaxMemoryKB { get; set; }

    public string[] GetArguments(string inputFile, string outputFile)
    {
        return GetArgs(inputFile, outputFile);
    }

    string[] GetArgs(string inputFile, string outputFile)
    {
        var args = new List<string>();

        if(Optimize)
        {
            args.Add("-optimize");
        }

        if(Progressive)
        {
            args.Add("-progressive");
        }

        if(RestartMcuRows != null)
        {
            args.Add("-restart");
            args.Add($"{RestartMcuRows}");
        }
        else if(RestartMcuBlocks != null)
        {
            args.Add("-restart");
            args.Add($"{RestartMcuBlocks}B");
        }

        if(Arithmetic)
        {
            args.Add("-arithmetic");
        }

        if(ScansFile != null)
        {
            if(!File.Exists(ScansFile))
            {
                throw new FileNotFoundException("ScansFile was not found!", ScansFile);
            }

            args.Add("-scans");
            args.Add(ScansFile);
        }

        switch(Flip)
        {
            case Flip.Horizontal:
                args.Add("-flip horizontal");
                break;
            case Flip.Vertical:
                args.Add("-flip vertical");
                break;
        }

        if(Rotate != Rotate.NotSpecified)
        {
            args.Add("-rotate");

            switch(Rotate)
            {
                case Rotate.Rotate90DegreesClockwise:
                    args.Add("90");
                    break;
                case Rotate.Rotate180Degrees:
                    args.Add("180");
                    break;
                case Rotate.Rotate90DegreesCounterClockwise:
                    args.Add("270");
                    break;
            }
        }

        if(Transpose)
        {
            args.Add("-transpose");
        }

        if(Transverse)
        {
            args.Add("-transverse");
        }

        if(Trim)
        {
            args.Add("-trim");
        }

        if(Perfect)
        {
            args.Add("-perfect");
        }

        if(CropSpecification != null)
        {
            if(!Regex.IsMatch(CropSpecification, @"\d+x\d+\+\d+\+\d+"))
            {
                throw new Exception("CropSpecification must be in the form WxH+X+Y!");
            }

            args.Add("-crop");
            args.Add(CropSpecification);
        }

        if(Grayscale)
        {
            args.Add("-grayscale");
        }

        if(Copy != Copy.NotSpecified)
        {
            args.Add("-copy");

            switch(Copy)
            {
                case Copy.None:
                    args.Add("none");
                    break;
                case Copy.Comments:
                    args.Add("comments");
                    break;
                case Copy.All:
                    args.Add("all");
                    break;
            }
        }

        if(MaxMemoryKB != null)
        {
            args.Add("-maxmemory");
            args.Add($"{MaxMemoryKB}");
        }

        if(!string.IsNullOrEmpty(outputFile))
        {
            args.Add("-outfile");
            args.Add(outputFile);
        }

        if(!string.IsNullOrEmpty(inputFile))
        {
            if(!File.Exists(inputFile))
            {
                throw new FileNotFoundException("Input file was not found!", inputFile);
            }

            args.Add(inputFile);
        }

        return args.ToArray();
    }
}
