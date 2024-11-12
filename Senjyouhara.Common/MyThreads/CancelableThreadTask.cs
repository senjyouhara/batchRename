
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Senjyouhara.Common.MyThreads;

public class CancelableThreadTask
{
    private System.Threading.Thread _thread;
    private readonly Action _action;
    private readonly Action<Exception> _onError;
    private readonly Action _onCompleted;
    private TaskCompletionSource<bool> _tcs;

    private int _isRunning = 0;

    public CancelableThreadTask(Action action, Action<Exception>? onError = null, Action? onCompleted = null)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _onError = onError;
        _onCompleted = onCompleted;
    }

    public Task RunAsync(CancellationToken token)
    {
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
            throw new InvalidOperationException("Task is already running");

        _tcs = new();
        _thread = new System.Threading.Thread(() =>
        {
            try
            {
                _action();
                _tcs.SetResult(true);
                _onCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                if (ex is ThreadInterruptedException)
                    _tcs.TrySetCanceled(token);
                else
                    _tcs.TrySetException(ex);
                _onError?.Invoke(ex);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
            }
        });

        token.Register(() =>
        {
            if (Interlocked.CompareExchange(ref _isRunning, 0, 1) == 1)
            {
                _thread.Interrupt();
                _thread.Join();
                _tcs.TrySetCanceled(token);
            }
        });

        _thread.Start();

        return _tcs.Task;
    }
}