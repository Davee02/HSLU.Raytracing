public delegate void RenderProgressCallback(float progress);
public class EnhancedProgressBar : IDisposable
{
    private readonly int _total;
    private readonly string _description;
    private readonly int _width;
    private readonly bool _showAnimation;
    private readonly RenderProgressCallback? _progressCallback;
    private int _current;
    private readonly DateTime _startTime;
    private readonly Lock _lock = new();
    private const string Animation = @"|/-\";
    private int _lastProgressBarLength = 0;

    public EnhancedProgressBar(int total, string description, int width = 30, bool showAnimation = true, RenderProgressCallback? progressCallback = null)
    {
        _total = total;
        _description = description;
        _width = width;
        _showAnimation = showAnimation;
        _progressCallback = progressCallback;
        _startTime = DateTime.Now;
        _current = 0;
        // Draw initial progress bar
        Draw();
    }

    public void Update(int current)
    {
        _lock.Enter();
        _current = current;
        Draw();
        // Also call the progress callback if provided
        float progress = (float)_current / _total;
        _progressCallback?.Invoke(progress);
        _lock.Exit();
    }

    private void Draw()
    {
        if (!Console.IsOutputRedirected)
        {
            // Calculate progress percentage
            float percent = (float)_current / _total;
            int completeWidth = (int)(_width * percent);

            // Time elapsed
            var elapsed = DateTime.Now - _startTime;

            // Animation frame
            var idx = (int)(DateTime.Now.Ticks / 3000000 % Animation.Length);
            var animationFrame = _showAnimation ? Animation[idx] : ' ';

            // Build the full progress bar string first
            string progressBar = $"{_description} [";

            for (int i = 0; i < _width; i++)
            {
                progressBar += (i < completeWidth) ? "█" : " ";
            }

            // Calculate estimated time remaining if progress is greater than 0
            string etaString = "";
            if (percent > 0)
            {
                var estimatedTotalTime = TimeSpan.FromTicks((long)(elapsed.Ticks / percent));
                var eta = estimatedTotalTime - elapsed;
                etaString = $" ETA: {FormatTimeSpan(eta)}";
            }

            progressBar += $"] {percent:P1} {animationFrame} [{elapsed:hh\\:mm\\:ss}]{etaString}";

            // Move cursor to beginning of line and write the progress bar
            Console.Write("\r" + progressBar);

            // Calculate current length to track for next update
            _lastProgressBarLength = progressBar.Length;

            // Make sure we don't add a newline
            Console.SetCursorPosition(Math.Min(progressBar.Length, Console.BufferWidth - 1), Console.CursorTop);
        }
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
        if (timeSpan.TotalMinutes >= 1)
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        return $"{timeSpan.Seconds}s";
    }

    public void Dispose()
    {
        if (!Console.IsOutputRedirected)
        {
            // Move to beginning of line
            Console.Write("\r");

            // Clear line by overwriting with spaces
            Console.Write(new string(' ', _lastProgressBarLength));

            // Move to beginning again
            Console.Write("\r");

            // Write final progress
            var elapsed = DateTime.Now - _startTime;
            Console.WriteLine("{0} completed in {1:hh\\:mm\\:ss}", _description, elapsed);
        }
    }
}