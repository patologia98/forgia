using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Forgia.UI.Services;

public class FileDialogService : IFileDialogService
{
    private static TopLevel GetTopLevel()
    {
        var lifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return TopLevel.GetTopLevel(lifetime!.MainWindow!)!;
    }

    public async Task<string?> OpenSliceFileAsync()
    {
        var files = await GetTopLevel().StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Slicer File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Bambu Slicer Files") { Patterns = ["*.gcode.3mf", "*.gcode"] },
                new FilePickerFileType("All Files") { Patterns = ["*"] },
            ],
        });

        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    public async Task<string?> SavePdfAsync(string suggestedFileName)
    {
        var file = await GetTopLevel().StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Quote PDF",
            SuggestedFileName = suggestedFileName,
            DefaultExtension = "pdf",
            FileTypeChoices = [new FilePickerFileType("PDF") { Patterns = ["*.pdf"] }],
        });

        return file?.Path.LocalPath;
    }
}
