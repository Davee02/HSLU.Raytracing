using System.Diagnostics;

namespace Common
{
    public class EnhancedProgressBar : IDisposable
    {
        private readonly int _totalTicks;
        private readonly int _barSize;
        private int _currentTick;
        private readonly Stopwatch _stopwatch;
        private bool _disposed;
        private readonly string _taskName;
        private readonly Timer _refreshTimer;
        private int _fps;
        private int _lastPercent;
        private readonly bool _showDetails;

        public EnhancedProgressBar(int totalTicks, string taskName = "Rendering", int barSize = 40, bool showDetails = true)
        {
            _totalTicks = totalTicks;
            _barSize = barSize;
            _currentTick = 0;
            _stopwatch = Stopwatch.StartNew();
            _taskName = taskName;
            _disposed = false;
            _fps = 0;
            _lastPercent = 0;
            _showDetails = showDetails;

            if (_showDetails)
            {
                Console.WriteLine($"Starting {_taskName}...");
                Console.WriteLine($"Total work items: {_totalTicks}");
                Console.WriteLine(new string('-', barSize + 20));
            }

            // Draw empty progress bar
            Console.Write("[");
            Console.Write(new string(' ', barSize));
            Console.Write("] 0%");

            // Start a timer to refresh the FPS counter independently of progress updates
            _refreshTimer = new Timer(UpdateFps, null, 1000, 1000);
        }

        private void UpdateFps(object? state)
        {
            if (_currentTick > 0 && !_disposed)
            {
                // Force a refresh of the display
                Update(_currentTick);
            }
        }

        public void Update(int tick)
        {
            _currentTick = tick;

            int percent = (int)Math.Floor((double)_currentTick / _totalTicks * 100);

            // Only update the display if the percentage changed or every second
            if (percent == _lastPercent && _currentTick < _totalTicks)
                return;

            _lastPercent = percent;

            int progressChars = (int)Math.Floor((double)_currentTick / _totalTicks * _barSize);

            // Calculate performance metrics
            TimeSpan elapsed = _stopwatch.Elapsed;
            _fps = (int)(_currentTick / elapsed.TotalSeconds);

            // Calculate estimated time remaining
            TimeSpan estimated = TimeSpan.FromSeconds(
                elapsed.TotalSeconds / _currentTick * (_totalTicks - _currentTick));

            string timeRemaining = _currentTick > 0
                ? $"ETA: {FormatTimeSpan(estimated)}"
                : "Calculating...";

            // Reset cursor to start of line and redraw
            ClearLastLine();
            Console.Write("[");
            Console.Write(new string('#', progressChars));
            Console.Write(new string(' ', _barSize - progressChars));
            Console.Write($"] {percent,3}% - {_fps,3} rows/sec - {timeRemaining}");

            // If complete
            if (_currentTick >= _totalTicks)
            {
                _refreshTimer.Dispose();
                Console.WriteLine();
                Console.WriteLine($"{_taskName} completed in {FormatTimeSpan(elapsed)}");
            }
        }

        private static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private static string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
            if (ts.TotalMinutes >= 1)
                return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
            return $"{ts.Seconds}s";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _refreshTimer?.Dispose();

                // Make sure we end with a new line if not already done
                if (_currentTick < _totalTicks)
                    Console.WriteLine();

                _disposed = true;
            }
        }
    }
}