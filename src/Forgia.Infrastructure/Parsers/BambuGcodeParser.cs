using System.Text.RegularExpressions;

namespace Forgia.Infrastructure.Parsers;

/// <summary>
/// Parses standalone Bambu Studio .gcode files (header comment block).
/// </summary>
public static partial class BambuGcodeParser
{
    // "; total estimated time: 1h 40m 26s"
    private static readonly Regex TimeRegex = BuildTimeRegex();

    // "; total filament weight [g] : 26.70"
    private static readonly Regex WeightRegex = BuildWeightRegex();

    public static Result<SliceData> Parse(string filePath)
    {
        if (!File.Exists(filePath))
            return Result<SliceData>.Fail($"File not found: {filePath}");

        try
        {
            return ParseHeader(filePath);
        }
        catch (Exception ex)
        {
            return Result<SliceData>.Fail($"Failed to parse {filePath}: {ex.Message}");
        }
    }

    private static Result<SliceData> ParseHeader(string filePath)
    {
        TimeSpan? printTime = null;
        decimal? filamentG = null;

        foreach (var line in File.ReadLines(filePath))
        {
            if (!line.StartsWith(';'))
                break; // header block ends at first non-comment line

            if (printTime is null)
            {
                var tm = TimeRegex.Match(line);
                if (tm.Success)
                    printTime = ParseTime(tm);
            }

            if (filamentG is null)
            {
                var wm = WeightRegex.Match(line);
                if (wm.Success &&
                    decimal.TryParse(wm.Groups["g"].Value,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var g))
                    filamentG = g;
            }

            if (printTime.HasValue && filamentG.HasValue)
                break;
        }

        if (printTime is null)
            return Result<SliceData>.Fail("Could not find 'total estimated time' in G-code header.");
        if (filamentG is null)
            return Result<SliceData>.Fail("Could not find 'total filament weight [g]' in G-code header.");

        return Result<SliceData>.Ok(new SliceData(printTime.Value, filamentG.Value));
    }

    private static TimeSpan ParseTime(Match m)
    {
        int.TryParse(m.Groups["h"].Value, out var h);
        int.TryParse(m.Groups["min"].Value, out var min);
        int.TryParse(m.Groups["s"].Value, out var s);
        return new TimeSpan(h, min, s);
    }

    [GeneratedRegex(
        @"total estimated time:\s*(?:(?<h>\d+)h\s*)?(?:(?<min>\d+)m\s*)?(?:(?<s>\d+)s)?",
        RegexOptions.IgnoreCase)]
    private static partial Regex BuildTimeRegex();

    [GeneratedRegex(
        @"total filament weight \[g\]\s*:\s*(?<g>[\d.]+)",
        RegexOptions.IgnoreCase)]
    private static partial Regex BuildWeightRegex();
}
