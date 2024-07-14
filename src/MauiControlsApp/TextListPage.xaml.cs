using Drastic.AllTheControls.ViewModels;

namespace MauiControlsApp;

public partial class TextListPage : ContentPage
{
    public TextListPage(TextListViewModel vm)
    {
        this.InitializeComponent();
        this.BindingContext = this.ViewModel = vm;
    }

    public TextListViewModel ViewModel { get; }
}