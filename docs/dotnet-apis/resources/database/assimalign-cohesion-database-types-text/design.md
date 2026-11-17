# Assimalign.Cohesion.Database.Types.Text design

Text is a concrete-first child root with sealed public leaves and one dependency, Database.Types.
It has no model, storage, hosting or language dependency. No index or query consumes it yet.

## Analysis and identity

The fixed pipeline tokenizes, optionally removes canonical accents, folds case, then for English
removes stop words and applies an in-house Porter2 stemmer. `simple` is the default. Both built-ins
are version 1, preserve accents by default and assign weight D. Descriptors persist exact,
case-sensitive Name, any int Version and FoldAccents. Creating a descriptor accepts unavailable
identities; TextAnalyzer.Create resolves them or raises UnknownAnalyzer/AnalyzerVersionMismatch.
There is no automatic upgrade. Output changes require a version bump and consumer refusal/rebuild.

## Token classes and folding

Unicode 17.0 Lu, Ll, Lt, Lm, Lo, Nl and Nd start words. Mn, Mc, Me and Cf continue an open word
but never start one. Other scalars separate words, including apostrophes. Adjacent letters and
digits form one token. There is no normalization before tokenization. Invalid surrogate sequences
raise InvalidUnicode. Classification tables are pinned in source rather than runtime tables.

Case folding uses Types' pinned Collation.CaseInsensitive; optional accent removal uses
Collation.CaseAccentInsensitive. Neither uses culture-sensitive casing, runtime normalization,
ICU or OS calls. FoldAccents removes every Mn/Mc/Me mark, including vowel signs: `काम` and `कम`
both become `कम`. It also decomposes Hangul syllables to conjoining jamo, increasing UTF-8 size.
Lexemes sort in Unicode scalar order, equivalent to UTF-8 byte order.

## English routing and PostgreSQL comparison

The stemmer follows PostgreSQL REL_17_11's Snowball 2.2.0 English algorithm. An observed corpus
of 60,379 dictionary pairs from PostgreSQL 17.11 checks suffix/exception behavior. No stemming
package or third-party vocabulary is used. The 127 stop words match REL_17_11's english.stop.
Stop words are removed before stemming, without rechecking the resulting stem. Empty stems keep
the input. Tokens containing ASCII 0–9 or exceeding 1,000 raw UTF-8 bytes bypass both stop-list
and stemmer and retain the folded form; non-ASCII Nd digits do not trigger that bypass.

PostgreSQL has specialized URL, mail, file, host, number, markup and hyphen token classes.
Text uses letter/digit runs with mark continuations. Representative simple-analysis differences:

| Input | Text behavior |
| --- | --- |
| `User.Name@example.com` | Four tokens: user, name, example, com; PostgreSQL retains the address |
| `state-of-the-art` | Four consecutive tokens; PostgreSQL also emits a whole hyphenated token |
| `4.50` | Two tokens, 4 and 50; PostgreSQL retains the number |
| `<title>Blue</title>` | title at positions 1/3 and blue at 2; PostgreSQL ignores markup |

Types uses Unicode default simple case folding rather than PostgreSQL simple lowercasing.
Final sigma folds to σ, long s to s and micro sign to μ; Cherokee folds to uppercase. Dotted
capital I remains dotted unless accents are removed. Sharp S to ß agrees with PostgreSQL.
Thus English `ſo` becomes the stop word `so` and disappears. Locale-specific PostgreSQL rules
are not part of Text's contract. Apostrophes split tokens even though the stemmer's possessive
step is independently checked.

## Values and limits

Values snapshot their inputs and expose read-only collections with structural equality.
Explicit lexemes do not tokenize, fold or stem; separators are allowed. Repeated lexemes merge.
Positions sort ascending and deduplicate, retaining the highest weight D < C < B < A. Excess
positions retain the earliest. All input positions must be positive; default TextPosition is invalid.

| Limit | Value |
| --- | --- |
| Explicit/folded lexeme | 2,047 UTF-8 bytes |
| Raw analyzer word | At most 2,046 bytes; longer words skip without consuming a position |
| Position | Values above 16,383 clamp |
| Explicit positions per lexeme | 256 |
| Analyzer positions per lexeme | 255 |
| Lexeme/position payload | 1,048,575 bytes; excludes datum header and lexeme directory |

A normalized word exceeding 2,047 bytes skips after consuming its position. Stop words leave
position gaps. Skipped words are silent; PostgreSQL emits notices. PostgreSQL's tsvector text
reader rejects a 2,047-byte literal, but analysis can fold an accepted 2,046-byte raw word into
a valid 2,047-byte value; Text models that value, not the text reader.

Raw UTF-8 length takes precedence over canonical-spelling agreement at parser/stemmer cutoffs.
For example, 400 NFC `é` plus `ing` is 803 bytes and stems after accent folding, while NFD is
1,203 bytes and bypasses stemming. Hangul expansion is subject to the folded lexeme limit.

Payload accounting aligns each positioned lexeme to two bytes, then adds a two-byte count and
two bytes per position. Builders bound retained positions and reject excessive payload before
the final sorted-layout alignment check. Duplicate positions always keep the highest weight,
including at position 16,383 and the 256th retained position. This deliberately differs from
PostgreSQL's unstable sort and early cutoff, which can retain a lower terminal weight.

## Conditions and notices

TextException.Code uses [TextErrorCode](text-error-code.md), separately registered under
[Text analysis conditions](../../../../database/error-codes.md#text-analysis-conditions).
No model diagnostic translation exists yet. Standard argument guards handle nulls, nonpositive
positions and invalid weights. Text is synchronous and owns no background process or EventSource.

The Snowball BSD-3 notice is retained in source headers and `docs/SNOWBALL_LICENSE.txt`.
It ships at `licenses/Database.Types.Text/SNOWBALL_LICENSE.txt` in Text's NuGet package,
both App.Database framework packs, and Sdk.Database, which bundles the assembly under Tasks/.
The generated Unicode table header cites Unicode License v3 in `docs/UNICODE_LICENSE.txt`;
that notice is currently not packed, following Types' precedent.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/TextAnalyzer.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/tests/TextUnicodeTests.cs`, `TextVectorTests.cs` and `Porter2StemmerTests.cs`.
