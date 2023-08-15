using Microsoft.Extensions.Logging;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// Extensions for logging.
/// </summary>
internal static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Critical,
        Message = "Failed to add item `{Key}` to the ResourceDictionary")]
    public static partial void FailedToAddKeyToResourceDictionary(this ILogger logger, object key, Exception exception);
}
