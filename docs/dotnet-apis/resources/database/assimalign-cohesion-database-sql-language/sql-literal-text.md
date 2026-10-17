# SqlLiteralText

Strict, invariant public readers for SQL literal contents, independent of the host clock and time
zone.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public static class SqlLiteralText
{
    public static DateOnly ParseDate(string text);
    public static TimeOnly ParseTime(string text);
    public static DateTime ParseTimestamp(string text);
    public static DateTimeOffset ParseTimestampWithTimeZone(string text);
    public static byte[] ParseBinary(string text);
    public static TimeSpan ParseInterval(string text, string? qualifier = null);
}
```

## Behavior and exceptions

Pass unquoted contents, not a full SQL literal. DATE requires yyyy-MM-dd; TIME requires HH:mm with
optional :ss and up to seven fractional digits. TIMESTAMP accepts a date and optional time separated
by one space or T, returns Unspecified kind, and rejects zones. Zoned timestamps allow Z or +/-HH:mm
within +/-14:00; omitted offset means UTC and written offsets are retained.

ParseBinary accepts even hexadecimal digits, including empty input. ParseInterval accepts day-time
qualified fields/ranges, PostgreSQL units/clocks or ISO durations at exact 100 ns resolution.
YEAR/MONTH fields throw NotSupportedException; invalid text throws FormatException; a duration
outside TimeSpan throws OverflowException. Null interval/binary input throws ArgumentNullException;
null date/time/timestamp input fails the respective grammar with FormatException.

The SQL parser converts malformed literal contents to SQL0008 and unsupported year-month forms to
COHDBL001. CAST reuses these strict readers, with CAST's own diagnostic mapping. String assignment
retains its separate SqlTemporalText input acceptance.

## Example

```csharp
var wallClock = SqlLiteralText.ParseTimestamp("2024-01-01 10:30");
Console.WriteLine(wallClock.Kind); // Unspecified
var duration = SqlLiteralText.ParseInterval("1:30", "HOUR TO MINUTE");
Console.WriteLine(duration.TotalMinutes); // 90
byte[] bytes = SqlLiteralText.ParseBinary("00ff"); // { 0, 255 }
```

## Related reference

[Assembly](index.md) · [SQL literals](../../../../database/sql/language/expressions/literals.md) · [CAST](../../../../database/sql/language/expressions/cast.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLiteralText.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTypedLiteralTests.cs`.
