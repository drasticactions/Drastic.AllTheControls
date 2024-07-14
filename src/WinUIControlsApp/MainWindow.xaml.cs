using Drastic.AllTheControls.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIControlsApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IServiceProvider serviceProvider;
        private List<SamplePageItem> listViewPages;

        public MainWindow(IServiceProvider provider)
        {
            this.InitializeComponent();
            this.serviceProvider = provider;
            this.SystemBackdrop = new MicaBackdrop();
            this.listViewPages = new List<SamplePageItem>() { 
                new SamplePageItem("TextListPage", typeof(ListViewPage)),
                new SamplePageItem("VariableHeightTextListPage", typeof(VariableHeightTextListPage)),
                new SamplePageItem("TextGridView", typeof(TextGridView))
            };
        }

        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                switch(item.Tag)
                {
                    case "ListView":
                        this.ControlListView.ItemsSource = this.listViewPages;
                        break;
                }
            }
        }

        private void ControlListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ControlListView.SelectedItem is SamplePageItem item)
            {
                switch(item.Name)
                {
                    case "TextListPage":
                        this.NavigationFrame.Content = this.serviceProvider.GetRequiredService<TextListPage>();
                        break;
                    case "VariableHeightTextListPage":
                        this.NavigationFrame.Content = this.serviceProvider.GetRequiredService<VariableHeightTextListPage>();
                        break;
                    case "TextGridView":
                        this.NavigationFrame.Content = this.serviceProvider.GetRequiredService<TextGridView>();
                        break;
                }
            }
            else
            {
                this.NavigationFrame.Content = null;
            }
        }
    }
}
