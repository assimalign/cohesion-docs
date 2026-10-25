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

Pass unquoted contents, not a full SQL literal. DATE requires yyyy-MM-dd. TIME accepts one- or
two-digit clock fields, AM/PM, up to seven fractional digits or a trailing dot. It validates and
discards a written numeric zone or supported abbreviation, preserving the clock. See the
[TIME zone vocabulary](../../../../database/sql/language/data-types/index.md#temporal-text-input).
TIMESTAMP accepts a date and optional time separated
by one space or T, returns Unspecified kind, and rejects zones. Zoned timestamps allow Z or +/-HH:mm
within +/-14:00; omitted offset means UTC and written offsets are retained.

ParseBinary accepts even hexadecimal digits, including empty input. ParseInterval accepts day-time
qualified fields/ranges, PostgreSQL units/clocks, bare signed seconds or ISO durations at exact
100 ns resolution. It accepts `1 2 hours`, optional `@` and final `ago` in unit/clock strings.
TIME/INTERVAL trim ASCII whitespace and reject non-ASCII whitespace. YEAR/MONTH fields throw
NotSupportedException; malformed text throws FormatException. Calendar, clock and offset fields
outside range now throw OverflowException, as does a duration outside TimeSpan.
Null interval/binary input throws ArgumentNullException;
null date/time/timestamp input fails the respective grammar with FormatException.

The SQL parser converts malformed literal contents to CS00008 and unsupported year-month forms to
CK00001. Assignment, DEFAULT and CAST share the temporal reader implementation and classify
bad format as CS02047 (22007), out-of-range fields as CS02048 (22008), and interval overflow
as CS02062 (22015). DATE/timestamp string assignment retains its established additional grammar.
Day-time TimeSpan results participate in
[temporal arithmetic and fields](../../../../database/sql/language/expressions/operators.md#temporal-arithmetic);
this does not add year/month intervals or a time-zone setting.

## Example

```csharp
using System;
using Assimalign.Cohesion.Database.Sql.Language;

var wallClock = SqlLiteralText.ParseTimestamp("2024-01-01 10:30");
Console.WriteLine(wallClock.Kind); // Unspecified
var duration = SqlLiteralText.ParseInterval("1:30", "HOUR TO MINUTE");
Console.WriteLine(duration.TotalMinutes); // 90
byte[] bytes = SqlLiteralText.ParseBinary("00ff"); // { 0, 255 }
Console.WriteLine(SqlLiteralText.ParseTime("1 PM").Hour); // 13
Console.WriteLine(SqlLiteralText.ParseTime("10:00+05:30").Hour); // 10
Console.WriteLine(SqlLiteralText.ParseInterval("12").TotalSeconds); // 12
Console.WriteLine(SqlLiteralText.ParseInterval("@ 1 day 2 hours ago").TotalHours); // -26
```

## Related reference

[Assembly](index.md) · [SQL literals](../../../../database/sql/language/expressions/literals.md) · [CAST](../../../../database/sql/language/expressions/cast.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/shared/SqlTemporalReader.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTypedLiteralTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTemporalAssignmentConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLiteralText.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTypedLiteralTests.cs`.
