using System.Globalization;

namespace TrelloCli.Utils;

public static class CardPosition
{
    public static bool TryRead(string[] args, out string? position, out string? error)
    {
        position = null;
        error = null;
        var firstOption = args[0] == "--update-card" ? 2 : 3;
        for (var i = firstOption; i < args.Length; i++)
        {
            if (args[i] != "--pos")
            {
                if (args[i] is "--name" or "--desc" or "--due" or "--labels" or "--members" or "--closed")
                    i++;
                continue;
            }
            if (position != null || i + 1 >= args.Length || !IsValid(args[i + 1]))
            {
                error = "--pos requires top, bottom, or a positive finite number (decimal point: .); specify it once";
                return false;
            }
            position = args[++i];
        }
        return true;
    }

    private static bool IsValid(string value) => value is "top" or "bottom" ||
        (double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number)
            && double.IsFinite(number) && number > 0);
}
