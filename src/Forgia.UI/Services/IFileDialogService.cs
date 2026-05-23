namespace Forgia.UI.Services;

public interface IFileDialogService
{
    Task<string?> OpenSliceFileAsync();
    Task<string?> SavePdfAsync(string suggestedFileName);
}
