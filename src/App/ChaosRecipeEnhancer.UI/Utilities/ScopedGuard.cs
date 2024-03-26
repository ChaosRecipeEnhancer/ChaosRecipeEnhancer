using System;

namespace ChaosRecipeEnhancer.UI.Utilities;

// REF: https://stackoverflow.com/a/31365171/10072406
public sealed class ScopeGuard : IDisposable
{
    private readonly Action _disposeAction;

    public ScopeGuard(Action disposeAction)
    {
        _disposeAction = disposeAction;
    }

    public void Dispose()
    {
        _disposeAction?.Invoke();
    }
}