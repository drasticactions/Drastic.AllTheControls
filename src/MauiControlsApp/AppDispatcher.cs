using Drastic.AppToolbox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiControlsApp;

internal class AppDispatcher : IAppDispatcher
{
    public bool Dispatch(Action action)
    {
        return Microsoft.Maui.Controls.Application.Current.Dispatcher.Dispatch(action);
    }
}
