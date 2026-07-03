using System;
using System.Windows.Threading;

namespace FolderNotifier.Services
{
    public class ShellWatcherService
    {
        private DispatcherTimer _timer;
        private string? _lastPath;
        private int _nullCount = 0;

        public event Action<string?>? FolderChanged;

        public void Start()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };

            _timer.Tick += (s, e) =>
            {
                var path = ExplorerHelper.GetActiveExplorerPath();

                if (path == "APP_FOCUSED")
                    return;

                if (path == null)
                {
                    _nullCount++;
                    if (_nullCount < 3)
                        return;
                }
                else
                {
                    _nullCount = 0;
                }

                if (path != _lastPath)
                {
                    _lastPath = path;
                    FolderChanged?.Invoke(path);
                }
            };

            _timer.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
        }
    }
}