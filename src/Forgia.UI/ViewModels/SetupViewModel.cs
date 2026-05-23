using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forgia.Domain.Entities;
using Forgia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forgia.UI.ViewModels;

public partial class SetupViewModel : ViewModelBase
{
    private readonly IDbContextFactory<ForgiaDbContext> _dbFactory;

    public SetupViewModel(IDbContextFactory<ForgiaDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public Action? SetupCompleted { get; set; }

    [ObservableProperty] private string _printerName = string.Empty;
    [ObservableProperty] private int _powerW = 500;
    [ObservableProperty] private decimal _purchaseCost = 1000m;
    [ObservableProperty] private decimal _usefulLifeH = 10000m;
    [ObservableProperty] private decimal _maintenanceCostPerH = 0.05m;

    [ObservableProperty] private string _spoolName = string.Empty;
    [ObservableProperty] private string _material = "PLA";
    [ObservableProperty] private decimal _costPerKg = 20m;
    [ObservableProperty] private decimal _currentStockG = 1000m;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;

    private bool CanSave =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(PrinterName) &&
        !string.IsNullOrWhiteSpace(SpoolName) &&
        PowerW > 0 &&
        PurchaseCost > 0 &&
        UsefulLifeH > 0;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync(CancellationToken ct)
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct).ConfigureAwait(false);
            db.Printers.Add(new Printer
            {
                Name = PrinterName.Trim(),
                PowerW = PowerW,
                PurchaseCost = PurchaseCost,
                UsefulLifeH = UsefulLifeH,
                MaintenanceCostPerH = MaintenanceCostPerH,
            });
            db.FilamentSpools.Add(new FilamentSpool
            {
                Name = SpoolName.Trim(),
                Material = Material.Trim(),
                CostPerKg = CostPerKg,
                CurrentStockG = CurrentStockG,
            });
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
            SetupCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnPrinterNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnSpoolNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnPowerWChanged(int value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnPurchaseCostChanged(decimal value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnUsefulLifeHChanged(decimal value) => SaveCommand.NotifyCanExecuteChanged();
}
