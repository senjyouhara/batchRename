
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Senjyouhara.Common.MyThreads;

public class CancelableProcessTask
{
    private readonly string _filename;
    private readonly string _arguments;

    private Process _process;
    private TaskCompletionSource<bool> _tcs;

    public Process Process
    {
        get
        {
            return _process;
        }
        private set
        {
            _process = value;
        }
    }


    private int _isRunning = 0;

    public CancelableProcessTask(string filename, string arguments)
    {
        _filename = filename;
        _arguments = arguments;
    }

    public Task RunAsync(CancellationToken token)
    {
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
            throw new InvalidOperationException("Task is already running");

        _tcs = new ();

        Process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _filename,
                Arguments = _arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        Process.Start();
        
        Process.EnableRaisingEvents = true;
        Process.Exited += (sender, args) =>
        {
            if (Process.ExitCode == 0)
                _tcs.SetResult(true);
            else
            {
                if (token.IsCancellationRequested)
                    _tcs.SetCanceled();
                else
                    _tcs.SetException(new Exception($"Process exited with code {Process.ExitCode}"));
            }
        };

        token.Register(() =>
        {
            if (Interlocked.CompareExchange(ref _isRunning, 0, 1) == 1)
            {
                Process.Kill();
            }
        });

        return _tcs.Task;
    }
}