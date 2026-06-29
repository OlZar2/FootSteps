using System.Globalization;

namespace FS.Application.AnnouncementLogic.DTOs;

public sealed record AdminAnimalAnnouncementCursor(DateTime CreatedAt, int ReportsCount)
{
    private const char Separator = ':';

    public string Encode()
    {
        var value = string.Create(
            CultureInfo.InvariantCulture,
            $"{CreatedAt.Ticks}{Separator}{ReportsCount}");

        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static bool TryDecode(string? encoded, out AdminAnimalAnnouncementCursor? cursor)
    {
        cursor = null;

        if (string.IsNullOrWhiteSpace(encoded))
            return true;

        try
        {
            var raw = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ToBase64(encoded)));
            var parts = raw.Split(Separator, StringSplitOptions.TrimEntries);

            if (parts.Length != 2
                || !long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks)
                || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var reportsCount))
                return false;

            cursor = new AdminAnimalAnnouncementCursor(new DateTime(ticks, DateTimeKind.Utc), reportsCount);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    private static string ToBase64(string value)
    {
        var base64 = value
            .Replace('-', '+')
            .Replace('_', '/');

        return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
    }
}
