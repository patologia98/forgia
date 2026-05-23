using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forgia.Domain.Entities;
using Forgia.Domain.Pricing;
using Forgia.Infrastructure.Parsers;
using Forgia.Infrastructure.Pdf;
using Forgia.Infrastructure.Persistence;
using Forgia.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Forgia.UI.ViewModels;

public partial class NewQuoteViewModel : ViewModelBase
{
    private readonly IDbContextFactory<ForgiaDbContext> _dbFactory;
    private readonly IFileDialogService _dialogs;

    public NewQuoteViewModel(IDbContextFactory<ForgiaDbContext> dbFactory, IFileDialogService dialogs)
    {
        _dbFactory = dbFactory;
        _dialogs = dialogs;
    }

    public ObservableCollection<Printer> Printers { get; } = [];
    public ObservableCollection<FilamentSpool> Spools { get; } = [];

    [ObservableProperty] private Printer? _selectedPrinter;
    [ObservableProperty] private FilamentSpool? _selectedSpool;
    [ObservableProperty] private string _selectedFilePath = string.Empty;
    [ObservableProperty] private string _parseError = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _customerName = string.Empty;
    [ObservableProperty] private decimal _electricityRate = 0.25m;
    [ObservableProperty] private decimal _wastePercent = 5m;
    [ObservableProperty] private decimal _marginPercent = 20m;
    [ObservableProperty] private decimal _vatPercent = 22m;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasBreakdown))] private PlateResult? _plateResult;
    [ObservableProperty] private OrderResult? _orderResult;

    private SliceData? _parsedData;

    public bool HasBreakdown => PlateResult is not null;

    public string PrintTimeDisplay => _parsedData is null ? "—" : _parsedData.PrintTime.ToString(@"h\h\ mm\m");
    public string FilamentDisplay => _parsedData is null ? "—" : $"{_parsedData.FilamentUsageG:F1} g";

    public async Task LoadDataAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct).ConfigureAwait(false);
        var printers = await db.Printers.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct).ConfigureAwait(false);
        var spools = await db.FilamentSpools.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct).ConfigureAwait(false);

        Printers.Clear();
        foreach (var p in printers) Printers.Add(p);
        Spools.Clear();
        foreach (var s in spools) Spools.Add(s);

        SelectedPrinter = Printers.FirstOrDefault();
        SelectedSpool = Spools.FirstOrDefault();
    }

    public void LoadFile(string path)
    {
        SelectedFilePath = path;
        ParseError = string.Empty;
        _parsedData = null;
        PlateResult = null;
        OrderResult = null;

        var result = path.EndsWith(".3mf", StringComparison.OrdinalIgnoreCase)
            ? BambuGcode3mfParser.Parse(path)
            : BambuGcodeParser.Parse(path);

        if (!result.IsSuccess)
        {
            ParseError = string.Format(Resources.Strings.ErrorParseFile, result.Error);
            return;
        }

        _parsedData = result.Value!;
        OnPropertyChanged(nameof(PrintTimeDisplay));
        OnPropertyChanged(nameof(FilamentDisplay));
        RecomputeQuote();
    }

    [RelayCommand]
    private async Task BrowseFileAsync(CancellationToken ct)
    {
        var path = await _dialogs.OpenSliceFileAsync();
        if (path is not null)
            LoadFile(path);
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportPdfAsync(CancellationToken ct)
    {
        var suggestedName = string.IsNullOrWhiteSpace(CustomerName)
            ? $"quote_{DateTime.Now:yyyyMMdd}.pdf"
            : $"quote_{CustomerName}_{DateTime.Now:yyyyMMdd}.pdf";

        // stay on UI thread for dialog, then capture values before going to thread pool
        var path = await _dialogs.SavePdfAsync(suggestedName);
        if (path is null) return;

        var order = new Order
        {
            Customer = new Customer { Name = string.IsNullOrWhiteSpace(CustomerName) ? "—" : CustomerName },
            CreatedAt = DateTime.Now,
            MarginRate = MarginPercent / 100m,
            VatRate = VatPercent / 100m,
            LaborActivities = [],
        };
        var plate = new Plate
        {
            PrintTime = _parsedData!.PrintTime,
            FilamentUsageG = _parsedData.FilamentUsageG,
            WasteRate = WastePercent / 100m,
        };
        var plateResult = PlateResult!;
        var orderResult = OrderResult!;
        var electricityRate = ElectricityRate;
        var printer = SelectedPrinter!;
        var spool = SelectedSpool!;

        IsBusy = true;
        try
        {
            await Task.Run(() =>
                QuoteExporter.ExportToFile(order, [(plate, printer, spool, plateResult)],
                    orderResult, electricityRate, path), ct);
        }
        catch (Exception ex)
        {
            ParseError = string.Format(Resources.Strings.ErrorExportFailed, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanExport => PlateResult is not null && OrderResult is not null && !IsBusy;

    private void RecomputeQuote()
    {
        if (_parsedData is null || SelectedPrinter is null || SelectedSpool is null)
        {
            PlateResult = null;
            OrderResult = null;
            ExportPdfCommand.NotifyCanExecuteChanged();
            return;
        }

        var plate = new Plate
        {
            PrintTime = _parsedData.PrintTime,
            FilamentUsageG = _parsedData.FilamentUsageG,
            WasteRate = WastePercent / 100m,
        };

        PlateResult = QuoteCalculator.CalculatePlate(plate, SelectedPrinter, SelectedSpool, ElectricityRate);
        OrderResult = QuoteCalculator.CalculateOrder([PlateResult], [], MarginPercent / 100m, VatPercent / 100m);
        ExportPdfCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedPrinterChanged(Printer? value) => RecomputeQuote();
    partial void OnSelectedSpoolChanged(FilamentSpool? value) => RecomputeQuote();
    partial void OnElectricityRateChanged(decimal value) => RecomputeQuote();
    partial void OnWastePercentChanged(decimal value) => RecomputeQuote();
    partial void OnMarginPercentChanged(decimal value) => RecomputeQuote();
    partial void OnVatPercentChanged(decimal value) => RecomputeQuote();
    partial void OnIsBusyChanged(bool value) => ExportPdfCommand.NotifyCanExecuteChanged();
}
