namespace ColorControl.Shared.EventDispatcher;

public static class EventExtensions
{
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e, CancellationToken token);

    public static Task InvokeAsync<TArgs>(this AsyncEventHandler<TArgs> func, object sender, TArgs e)
    {
        return func == null ? Task.CompletedTask
            : Task.WhenAll(func.GetInvocationList().Cast<AsyncEventHandler<TArgs>>().Select(f => f(sender, e, CancellationToken.None)));
    }
}
