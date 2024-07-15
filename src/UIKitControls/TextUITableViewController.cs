using Drastic.AllTheControls.ViewModels;
using Drastic.AppToolbox.Data;

namespace UIKitControls;

public class TextUITableViewController : UITableViewController
{
    public TextUITableViewController(TextListViewModel vm) : base(UITableViewStyle.Plain)
    {
        this.TableView!.DataSource = new TextUITableDataSource(vm);
    }
    
    public class TextUITableDataSource : UITableViewDataSource
    {
        private TextListViewModel _viewModel;
        
        public TextUITableDataSource(TextListViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("cell") ?? new UITableViewCell(UITableViewCellStyle.Default, "cell");
            var record = _viewModel.TextRecords.Data[indexPath.Row];
            cell.TextLabel.Text = record.Text;
            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _viewModel.TextRecords.Data.Count;
        }
    }
    
}

public class TextBindUITableViewController : UITableViewController
{
    public TextBindUITableViewController(TextListViewModel vm) : base(UITableViewStyle.Plain)
    {
        vm.TextRecords.SetCellProvider(this.TableView, new TextRecordCellProvider());
        vm.TextRecords.Bind(this.TableView!);
    }
    
    public class TextRecordCellProvider : ITableCellProvider<TextRecord>
    {
        public UITableViewCell GetCell(UITableView tableView, TextRecord item)
        {
            var cell =tableView.DequeueReusableCell("TextRecordCell") ?? new UITableViewCell(UITableViewCellStyle.Default, "TextRecordCell");
            cell.TextLabel.Text = item.Text;
            return cell;
        }

        public float GetHeightForRow(NSIndexPath indexPath, TextRecord item)
        {
            return 33f;
        }
    }
}