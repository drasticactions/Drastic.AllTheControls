using Drastic.AppToolbox.Services;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUIControlsApp;

internal class AppDispatcher : IAppDispatcher
{
    private DispatcherQueue dispatcherQueue;

    public AppDispatcher(DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
    }

    public bool Dispatch(Action action)
    {
        _ = action ?? throw new ArgumentNullException(nameof(action));
        return this.dispatcherQueue.TryEnqueue(() => action());
    }
}
