using Avalonia.Controls;
using Avalonia.Input;
using Forgia.UI.ViewModels;

namespace Forgia.UI.Views;

public partial class NewQuoteView : UserControl
{
    public NewQuoteView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        DragDrop.SetAllowDrop(this, true);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(DataFormats.Files)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not NewQuoteViewModel vm) return;
        var files = e.Data.GetFiles();
        var first = files?.FirstOrDefault();
        if (first is not null)
            vm.LoadFile(first.Path.LocalPath);
        e.Handled = true;
    }
}
