using Drastic.AllTheControls.Model;

namespace MauiControlsApp;

public partial class MainPage : ContentPage
{
	int count = 0;

    private IServiceProvider serviceProvider;

    private List<SamplePageGroup> samplePageItems;

    public MainPage(IServiceProvider provider)
	{
		InitializeComponent();
        this.serviceProvider = provider;
        this.samplePageItems = new List<SamplePageGroup>();
        this.samplePageItems.Add(new SamplePageGroup("ListView", new List<SamplePageItem>() { 
            new SamplePageItem("Text List", typeof(TextListPage)),
        }));
        this.MainCollectionView.ItemsSource = this.samplePageItems;
    }

    private void MainCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SamplePageItem item)
        {
            switch (item.Name)
            {
                case "Text List":
                    this.Navigation.PushAsync(this.serviceProvider.GetRequiredService<TextListPage>());
                    break;
            }

            this.MainCollectionView.SelectedItem = null;
        }
    }
}

public class SamplePageGroup : List<SamplePageItem>
{
    public string Name { get; private set; }

    public SamplePageGroup(string name, List<SamplePageItem> animals) : base(animals)
    {
        Name = name;
    }
}

