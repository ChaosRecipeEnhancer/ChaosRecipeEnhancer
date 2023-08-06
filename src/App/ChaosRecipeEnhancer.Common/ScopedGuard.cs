namespace ChaosRecipeEnhancer.Common;

public sealed class ScopeGuard : IDisposable
{
	private readonly Action _disposeAction;

	public ScopeGuard(Action disposeAction) => _disposeAction = disposeAction;

	public void Dispose() => _disposeAction?.Invoke();
}