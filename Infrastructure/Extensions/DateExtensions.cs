namespace Infrastructure.Extensions;

public static class DateExtensions
{
    public static string? ToIsoDate(this DateTime? date)
    {
        return date?.ToString("yyyy-MM-dd");
    }
    public static string ToIsoDate(this DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }
}