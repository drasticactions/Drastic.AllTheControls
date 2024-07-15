using Bogus;
using Drastic.AppToolbox.Data;
using Drastic.AppToolbox.Services;
using Drastic.AppToolbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AllTheControls.ViewModels;

public class TextListViewModel : BaseViewModel
{
    private Faker<TextRecord> _textRecordFaker;

    public TextListViewModel(
        IAppDispatcher dispatcher,
        IErrorHandler errorHandler,
        IAsyncCommandFactory asyncCommandFactory) 
        : base(
            dispatcher,
            errorHandler,
            asyncCommandFactory)
    {
        _textRecordFaker = new Faker<TextRecord>()
           .CustomInstantiator(f => new TextRecord(f.Lorem.Paragraphs(1, 5)));
        this.TextRecords = new ObservableDataSource<TextRecord>(this._textRecordFaker.Generate(100));
    }

    public ObservableDataSource<TextRecord> TextRecords { get; }
}

public record TextRecord(string Text);
