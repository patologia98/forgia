using CommunityToolkit.Mvvm.ComponentModel;
using Forgia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Forgia.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDbContextFactory<ForgiaDbContext> _dbFactory;
    private readonly SetupViewModel _setupVm;
    private readonly NewQuoteViewModel _quoteVm;

    public MainWindowViewModel(
        IDbContextFactory<ForgiaDbContext> dbFactory,
        SetupViewModel setupVm,
        NewQuoteViewModel quoteVm)
    {
        _dbFactory = dbFactory;
        _setupVm = setupVm;
        _quoteVm = quoteVm;

        _setupVm.SetupCompleted = () => _ = ShowQuoteAsync();
        _currentViewModel = _setupVm;
    }

    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct).ConfigureAwait(false);
        if (await db.Printers.AnyAsync(ct).ConfigureAwait(false))
            await ShowQuoteAsync(ct).ConfigureAwait(false);
        else
            CurrentViewModel = _setupVm;
    }

    private async Task ShowQuoteAsync(CancellationToken ct = default)
    {
        await _quoteVm.LoadDataAsync(ct).ConfigureAwait(false);
        CurrentViewModel = _quoteVm;
    }
}
