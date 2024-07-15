using Drastic.AllTheControls.ViewModels;
using Drastic.AppToolbox.Services;
using Microsoft.Extensions.DependencyInjection;
using MonoTouch.Dialog;

namespace UIKitControls;

public class SidebarItem(string title, SidebarItemType itemType) : NSObject
{
    public string Title => title;
    
    public SidebarItemType ItemType => itemType;
}

public enum SidebarItemType
{
    Other,
    Row
}

public class RootViewController : UISplitViewController
{
    private IServiceProvider serviceProvider;
    private UIViewController sidebarViewController;
    private UIViewController detailViewController;

    public RootViewController()
        : base(UISplitViewControllerStyle.DoubleColumn)
    {
        this.serviceProvider = new ServiceCollection()
            .AddSingleton<IAppDispatcher, AppDispatcher>()
            .AddSingleton<IErrorHandler, ErrorHandler>()
            .AddSingleton<IAsyncCommandFactory, AsyncCommandFactory>()
            .AddSingleton<TextListViewModel>()
            .AddSingleton<TextUITableViewController>()
            .AddSingleton<TextBindUITableViewController>()
            .AddKeyedSingleton<UIViewController>("DefaultBasicViewController", new BasicViewController())
            .AddKeyedSingleton<UIViewController>("DefaultDialogViewController", new DialogViewController(DialogViewGenerator.GenerateDefault()))
            .BuildServiceProvider();

        this.sidebarViewController = new SidebarViewController(this.OnSidebarItemSelected);
        this.detailViewController = new BasicViewController();
        this.SetViewController(this.sidebarViewController, UISplitViewControllerColumn.Primary);
        this.SetViewController(this.detailViewController, UISplitViewControllerColumn.Secondary);
        this.PreferredDisplayMode = UISplitViewControllerDisplayMode.OneBesideSecondary;
#if !TVOS
        this.PreferredPrimaryColumnWidth = 245f;
        this.PrimaryBackgroundStyle = UISplitViewControllerBackgroundStyle.Sidebar;
#endif
    }
    
    private void OnSidebarItemSelected(SidebarItem? item)
    {
        this.SetViewController(null, UISplitViewControllerColumn.Secondary);
        if (item is null)
        {
            return;
        }

        switch (item.Title)
        {
            case "Basic MD Sample":
                this.SetViewController(this.serviceProvider.GetRequiredKeyedService<UIViewController>("DefaultDialogViewController"), UISplitViewControllerColumn.Secondary);
                break;
            case "Basic Sample":
                this.SetViewController(this.serviceProvider.GetRequiredKeyedService<UIViewController>("DefaultBasicViewController"), UISplitViewControllerColumn.Secondary);
                break;
            case "TextListView - UITableView":
                this.SetViewController(this.serviceProvider.GetRequiredService<TextUITableViewController>(), UISplitViewControllerColumn.Secondary);
                break;
            case "TextListView - Bind - UITableView":
                this.SetViewController(this.serviceProvider.GetRequiredService<TextBindUITableViewController>(), UISplitViewControllerColumn.Secondary);
                break;
        }
    }
}

public static class DialogViewGenerator
{
    public static RootElement GenerateDefault()
    {
        return new RootElement("Settings")
        {
            new Section()
            {
                new BooleanElement("Airplane Mode", false),
                new RootElement("Notifications", 0, 0)
                {
                    new Section(null,
                        "Turn off Notifications to disable Sounds\n" +
                        "Alerts and Home Screen Badges for the\napplications below.")
                    {
                        new BooleanElement("Notifications", false)
                    }
                }
            },
            new Section()
            {
                new RootElement("Sounds")
                {
                    new Section("Silent")
                    {
                        new BooleanElement("Vibrate", true),
                    },
                    new Section("Ring")
                    {
                        new BooleanElement("Vibrate", true),
                        new FloatElement(null, null, 0.8f),
                        new RootElement("Ringtone", new RadioGroup(0))
                        {
                            new Section("Custom")
                            {
                                new RadioElement("Circus Music"),
                                new RadioElement("True Blood"),
                            },
                            new Section("Standard")
                            {
                                from name in "Marimba,Alarm,Ascending,Bark".Split(',')
                                select (Element)new RadioElement(name)
                            }
                        },
                        new RootElement("New Text Message", new RadioGroup(3))
                        {
                            new Section()
                            {
                                from name in "None,Tri-tone,Chime,Glass,Horn,Bell,Electronic".Split(',')
                                select (Element)new RadioElement(name)
                            }
                        },
                        new BooleanElement("New Voice Mail", false),
                        new BooleanElement("New Mail", false),
                        new BooleanElement("Sent Mail", true),
                    }
                },
                new RootElement("Brightness")
                {
                    new Section()
                    {
                        new FloatElement(null, null, 0.5f),
                        new BooleanElement("Auto-brightness", false),
                    }
                },
            },
            new Section()
            {
                new EntryElement("Login", "Your login name", "miguel"),
                new EntryElement("Password", "Your password", "password", true),
                new DateElement("Select Date", DateTime.Now),
            },
        };
    }
}

public class SidebarViewController : UIViewController, IUICollectionViewDelegate
{
    private UICollectionView collectionView;
    private Action<SidebarItem?> onSidebarItemSelected; 
    private UICollectionViewDiffableDataSource<NSString, SidebarItem>? dataSource;

    public SidebarViewController(Action<SidebarItem?> onSidebarItemSelected)
    {
        this.onSidebarItemSelected = onSidebarItemSelected;
        this.collectionView = new UICollectionView(this.View!.Bounds, this.CreateLayout());
        this.collectionView.Delegate = this;

        this.View.AddSubview(this.collectionView);
        this.collectionView.TranslatesAutoresizingMaskIntoConstraints = false;

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            this.collectionView.TopAnchor.ConstraintEqualTo(this.View.TopAnchor),
            this.collectionView.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor),
            this.collectionView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
            this.collectionView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
        });
        
        this.ConfigureDataSource();
        this.Generate();
    }

    private void Generate()
    {
        var basicItemSnapshot = new NSDiffableDataSourceSectionSnapshot<SidebarItem>();
        var basicItem = new SidebarItem("Basic", SidebarItemType.Other);
        basicItemSnapshot.AppendItems(new[] { basicItem });
        basicItemSnapshot.ExpandItems(new[] { basicItem });
        basicItemSnapshot.AppendItems(new[] { new SidebarItem("Basic Sample", SidebarItemType.Row) }, basicItem);
        this.dataSource!.ApplySnapshot(basicItemSnapshot, new NSString(Guid.NewGuid().ToString()), true);

        var dialogItemSnapshot = new NSDiffableDataSourceSectionSnapshot<SidebarItem>();
        var dialogItem = new SidebarItem("Monotouch.Dialog", SidebarItemType.Other);
        dialogItemSnapshot.AppendItems(new[] { dialogItem });
        dialogItemSnapshot.ExpandItems(new[] { dialogItem });
        dialogItemSnapshot.AppendItems(new[] { new SidebarItem("Basic MD Sample", SidebarItemType.Row) }, dialogItem);
        this.dataSource!.ApplySnapshot(dialogItemSnapshot, new NSString(Guid.NewGuid().ToString()), true);
        
        var tableViewItemSnapshot = new NSDiffableDataSourceSectionSnapshot<SidebarItem>();
        var tableItem = new SidebarItem("UITableView", SidebarItemType.Other);
        tableViewItemSnapshot.AppendItems(new[] { tableItem });
        tableViewItemSnapshot.ExpandItems(new[] { tableItem });
        tableViewItemSnapshot.AppendItems(new[] { new SidebarItem("TextListView - UITableView", SidebarItemType.Row) }, tableItem);
        tableViewItemSnapshot.AppendItems(new[] { new SidebarItem("TextListView - Bind - UITableView", SidebarItemType.Row) }, tableItem);
        this.dataSource!.ApplySnapshot(tableViewItemSnapshot, new NSString(Guid.NewGuid().ToString()), true);
    }
    
    /// <summary>
    /// Fired when Item is Selected.
    /// </summary>
    /// <param name="collectionView">CollectionView.</param>
    /// <param name="indexPath">Index Path.</param>
    [Export("collectionView:didSelectItemAtIndexPath:")]
    protected void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var item = this.dataSource?.GetItemIdentifier(indexPath);
        this.onSidebarItemSelected(item);
    }

    private void ConfigureDataSource()
    {
        var headerRegistration = UICollectionViewCellRegistration.GetRegistration(
            typeof(UICollectionViewListCell),
            new UICollectionViewCellRegistrationConfigurationHandler((cell, indexpath, item) =>
            {
                var sidebarItem = (SidebarItem)item;
#if !TVOS
                var contentConfiguration = UIListContentConfiguration.SidebarHeaderConfiguration;
#else
                var contentConfiguration = UIListContentConfiguration.GroupedHeaderConfiguration;
#endif
                contentConfiguration.Text = sidebarItem.Title;
                contentConfiguration.TextProperties.Font = UIFont.PreferredSubheadline;
                contentConfiguration.TextProperties.Color = UIColor.SecondaryLabel;
                cell.ContentConfiguration = contentConfiguration;
            }));
        
        var rowRegistration = UICollectionViewCellRegistration.GetRegistration(
            typeof(UICollectionViewListCell),
            new UICollectionViewCellRegistrationConfigurationHandler((cell, indexpath, item) =>
            {
                var sidebarItem = item as SidebarItem;
                if (sidebarItem is null)
                {
                    return;
                }

#if TVOS
                var cfg = UIListContentConfiguration.CellConfiguration;
#else
                var cfg = UIListContentConfiguration.SidebarCellConfiguration;
#endif
                cfg.Text = sidebarItem.Title;

                cell.ContentConfiguration = cfg;
            }));

        if (this.collectionView is null)
        {
            throw new NullReferenceException(nameof(this.collectionView));
        }

        this.dataSource = new UICollectionViewDiffableDataSource<NSString, SidebarItem>(
            this.collectionView,
            new UICollectionViewDiffableDataSourceCellProvider((collectionView, indexPath, item) =>
            {
                var sidebarItem = item as SidebarItem;
                if (sidebarItem is null || collectionView is null)
                {
                    throw new Exception();
                }

                return sidebarItem.ItemType switch
                {
                    SidebarItemType.Other => collectionView.DequeueConfiguredReusableCell(headerRegistration, indexPath, item),
                    _ => collectionView.DequeueConfiguredReusableCell(rowRegistration, indexPath, item),
                };
            })
        );
    }

    private UICollectionViewLayout CreateLayout()
    {
        return new UICollectionViewCompositionalLayout((sectionIndex, layoutEnvironment) =>
        {
#if TVOS
            var configuration = new UICollectionLayoutListConfiguration(UICollectionLayoutListAppearance.Grouped);
            configuration.HeaderMode = UICollectionLayoutListHeaderMode.FirstItemInSection;
            return NSCollectionLayoutSection.GetSection(configuration, layoutEnvironment);
#else
            var configuration = new UICollectionLayoutListConfiguration(UICollectionLayoutListAppearance.Sidebar);
            configuration.ShowsSeparators = true;
            configuration.HeaderMode = UICollectionLayoutListHeaderMode.FirstItemInSection;
            return NSCollectionLayoutSection.GetSection(configuration, layoutEnvironment);
#endif
        });
    }
}

public class BasicViewController : UIViewController
{
    public BasicViewController()
    {
        this.View!.AddSubview(new UILabel(View!.Frame)
        {
#if !TVOS
            BackgroundColor = UIColor.SystemBackground,
#endif
            TextAlignment = UITextAlignment.Center,
            Text = "Hello, Apple!",
            AutoresizingMask = UIViewAutoresizing.All,
        });
    }
}

public class AppDispatcher : NSObject, IAppDispatcher
{
    public bool Dispatch(Action action)
    {
        this.InvokeOnMainThread(action);
        return true;
    }
}

public class ErrorHandler : IErrorHandler
{
    public void HandleError(Exception ex)
    {
    }
}