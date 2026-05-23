using System.IO.Compression;
using System.Xml.Linq;

namespace Forgia.Infrastructure.Parsers;

/// <summary>
/// Parses Bambu Studio .gcode.3mf files (ZIP archive containing slice_info.config).
/// </summary>
public static class BambuGcode3mfParser
{
    private const string SliceInfoPath = "Metadata/slice_info.config";

    public static Result<SliceData> Parse(string filePath)
    {
        if (!File.Exists(filePath))
            return Result<SliceData>.Fail($"File not found: {filePath}");

        try
        {
            using var archive = ZipFile.OpenRead(filePath);
            var entry = archive.GetEntry(SliceInfoPath);
            if (entry is null)
                return Result<SliceData>.Fail($"{SliceInfoPath} not found in archive.");

            using var stream = entry.Open();
            var doc = XDocument.Load(stream);
            return ParseSliceInfo(doc);
        }
        catch (Exception ex)
        {
            return Result<SliceData>.Fail($"Failed to parse {filePath}: {ex.Message}");
        }
    }

    private static Result<SliceData> ParseSliceInfo(XDocument doc)
    {
        var plate = doc.Root?.Element("plate");
        if (plate is null)
            return Result<SliceData>.Fail("No <plate> element in slice_info.config.");

        var metaDict = plate.Elements("metadata")
            .ToDictionary(
                e => e.Attribute("key")?.Value ?? string.Empty,
                e => e.Attribute("value")?.Value ?? string.Empty);

        if (!metaDict.TryGetValue("prediction", out var predStr) ||
            !int.TryParse(predStr, out var predSeconds))
            return Result<SliceData>.Fail("Missing or invalid 'prediction' in slice_info.config.");

        if (!metaDict.TryGetValue("weight", out var weightStr) ||
            !decimal.TryParse(weightStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var weightG))
            return Result<SliceData>.Fail("Missing or invalid 'weight' in slice_info.config.");

        return Result<SliceData>.Ok(new SliceData(TimeSpan.FromSeconds(predSeconds), weightG));
    }
}
