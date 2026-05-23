using Forgia.Infrastructure.Parsers;
using FluentAssertions;

namespace Forgia.Infrastructure.Tests.Parsers;

public class BambuGcode3mfParserTests
{
    private static string SamplePath =>
        Path.Combine(AppContext.BaseDirectory, "Classic_Ikea_handle_(BESTA).gcode.3mf");

    [Fact]
    public void Parse_ValidBambuGcode3mf_ReturnsCorrectPrintTime()
    {
        var result = BambuGcode3mfParser.Parse(SamplePath);

        result.IsSuccess.Should().BeTrue(result.Error);
        // slice_info.config: prediction=6026 seconds = 1h 40m 26s
        result.Value!.PrintTime.Should().Be(TimeSpan.FromSeconds(6026));
    }

    [Fact]
    public void Parse_ValidBambuGcode3mf_ReturnsCorrectFilamentWeight()
    {
        var result = BambuGcode3mfParser.Parse(SamplePath);

        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value!.FilamentUsageG.Should().Be(26.70m);
    }

    [Fact]
    public void Parse_NonExistentFile_ReturnsFailure()
    {
        var result = BambuGcode3mfParser.Parse("/tmp/does_not_exist.gcode.3mf");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
