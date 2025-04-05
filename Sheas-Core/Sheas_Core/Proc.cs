namespace Sheas_Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    public class Proc : IDisposable
    {
        private readonly string _fileName;
        private Process _process;
        private bool _disposed;

        public Proc(string fileName)
        {
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        protected virtual void OnErrorDataReceived(DataReceivedEventArgs e)
        {
        }

        protected virtual void OnExited(EventArgs e)
        {
        }

        protected virtual void OnOutputDataReceived(DataReceivedEventArgs e)
        {
        }

        public async Task<int> RunAsync(string directory, string arguments)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                directory = Environment.GetFolderPath(Environment.SpecialFolder.System);
            }

            _process = new Process
            {
                StartInfo = {
                    FileName = _fileName,
                    WorkingDirectory = directory,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            _process.OutputDataReceived += (sender, e) => OnOutputDataReceived(e);
            _process.ErrorDataReceived += (sender, e) => OnErrorDataReceived(e);
            _process.Exited += (sender, e) => OnExited(e);

            try
            {
                _process.Start();
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();

                await Task.Run(() => _process.WaitForExit());
                return _process.ExitCode;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException)
            {
                try
                {
                    _process.StartInfo.Verb = "RunAs";
                    _process.Start();
                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();

                    await Task.Run(() => _process.WaitForExit());
                    return _process.ExitCode;
                }
                catch (Exception innerEx)
                {
                    throw new Exception($"{innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _process != null)
                {
                    try
                    {
                        if (!_process.HasExited)
                        {
                            _process.Kill();
                        }
                    }
                    catch { }
                    finally
                    {
                        _process.Dispose();
                        _process = null;
                    }
                }
                _disposed = true;
            }
        }

        ~Proc()
        {
            Dispose(false);
        }
    }
}