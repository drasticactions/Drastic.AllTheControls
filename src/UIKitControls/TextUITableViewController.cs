using Drastic.AllTheControls.ViewModels;

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
            var record = _viewModel.TextRecords[indexPath.Row];
            cell.TextLabel.Text = record.Text;
            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _viewModel.TextRecords.Count;
        }
    }
    
}