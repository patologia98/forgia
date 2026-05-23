using System.IO.Compression;
using Forgia.Infrastructure.Parsers;
using FluentAssertions;

namespace Forgia.Infrastructure.Tests.Parsers;

public class BambuGcodeParserTests : IDisposable
{
    private readonly string _gcodeFile;

    public BambuGcodeParserTests()
    {
        var archive = Path.Combine(AppContext.BaseDirectory, "Classic_Ikea_handle_(BESTA).gcode.3mf");
        _gcodeFile = Path.GetTempFileName();
        using var zip = ZipFile.OpenRead(archive);
        zip.GetEntry("Metadata/plate_1.gcode")!.ExtractToFile(_gcodeFile, overwrite: true);
    }

    public void Dispose() => File.Delete(_gcodeFile);

    [Fact]
    public void Parse_ValidBambuGcode_ReturnsCorrectPrintTime()
    {
        var result = BambuGcodeParser.Parse(_gcodeFile);

        result.IsSuccess.Should().BeTrue(result.Error);
        // header: "; total estimated time: 1h 40m 26s"
        result.Value!.PrintTime.Should().Be(new TimeSpan(1, 40, 26));
    }

    [Fact]
    public void Parse_ValidBambuGcode_ReturnsCorrectFilamentWeight()
    {
        var result = BambuGcodeParser.Parse(_gcodeFile);

        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value!.FilamentUsageG.Should().Be(26.70m);
    }

    [Fact]
    public void Parse_NonExistentFile_ReturnsFailure()
    {
        var result = BambuGcodeParser.Parse("/tmp/ghost.gcode");

        result.IsSuccess.Should().BeFalse();
    }
}
