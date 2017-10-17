[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/AerisG222/NJpegTran/blob/master/LICENSE.md)
[![NuGet](https://buildstats.info/nuget/NJpegTran)](https://www.nuget.org/packages/NJpegTran/)
[![Travis](https://img.shields.io/travis/AerisG222/NJpegTran.svg)](https://travis-ci.org/AerisG222/NJpegTran)
[![Coverity Scan](https://img.shields.io/coverity/scan/xxxx.svg)](https://scan.coverity.com/projects/aerisg222-njpegtran)

# NJpegTran

A .Net library to wrap the functionality of jpegtran.

## Motivation
To create a simple wrapper to allow .Net applications to easily use this program.

## Using
- Install libjpeg-turbo-progs
- Add a package reference to NJpegTran in your project
- Bring down the packages for your project via `dotnet restore`

Use it:

````c#
var opts = new Options {
    Optimize = true,
    Progressive = true,
    Copy = Copy.None
};

var jt = new JpegTran(opts);
var result = await jt.RunAsync("src.jpg", "dst.jpg");
````

## Contributing
I'm happy to accept pull requests.  By submitting a pull request, you
must be the original author of code, and must not be breaking
any laws or contracts.

Otherwise, if you have comments, questions, or complaints, please file
issues to this project on the github repo.
  
## License
NJpegTran is licensed under the MIT license.  See LICENSE.md for more
information.

## Reference
- JpegTran: [https://github.com/libjpeg-turbo/libjpeg-turbo](https://github.com/libjpeg-turbo/libjpeg-turbo)
