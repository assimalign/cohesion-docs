# OqlSelectExpression

A query over a collection, derived SELECT or expression, with duplicate removal and bare-value output.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlSelectExpression : OqlQueryExpression
{
    public OqlSelectExpression(string collection, string? alias,
        IReadOnlyList<OqlProjection> projections, OqlExpression? predicate,
        IReadOnlyList<OqlExpression> groupBy, OqlExpression? having,
        IReadOnlyList<OqlOrdering> orderBy, bool isDistinct, bool isValue,
        OqlExpression? limit = null, OqlExpression? offset = null, Location? location = null,
        IReadOnlyList<OqlUnnestExpression>? unnest = null,
        IReadOnlyList<string?>? groupAliases = null, string? groupAs = null, OqlExpression? source = null,
        IReadOnlyList<OqlSourceBindingExpression>? bindings = null, bool isAliasImplicit = false,
        IReadOnlyList<OqlWindowDefinition>? windows = null);
    public string Collection { get; }
    public OqlExpression? Source { get; }
    public string? Alias { get; }
    public bool IsAliasImplicit { get; }
    public IReadOnlyList<OqlUnnestExpression> Unnest { get; }
    public IReadOnlyList<OqlSourceBindingExpression> Bindings { get; }
    public IReadOnlyList<OqlProjection> Projections { get; }
    public OqlExpression? Predicate { get; }
    public IReadOnlyList<OqlExpression> GroupBy { get; }
    public IReadOnlyList<string?> GroupAliases { get; }
    public string? GroupAs { get; }
    public OqlExpression? Having { get; }
    public IReadOnlyList<OqlWindowDefinition> Windows { get; }
    public IReadOnlyList<OqlOrdering> OrderBy { get; }
    public bool IsDistinct { get; }
    public bool IsValue { get; }
    public OqlExpression? Limit { get; }
    public OqlExpression? Offset { get; }
}
```

## Behavior and exceptions

This sealed leaf inherits OrderBy, IsValue, Limit and Offset from
[OqlQueryExpression](oql-query-expression.md); the signature above includes these inherited members.

IsDistinct controls duplicate removal after projection and before ordering. IsValue represents
VALUE and RAW, which share one bare-value result with metadata column `value`. A top-level MISSING
becomes NULL before deduplication and counting on every transport. Named projections keep MISSING.
DISTINCT uses structural collation equality and retains the first representative; objects containing
NULL can deduplicate. See [SELECT](../../../../database/document/language/statements/select.md).

Limit and Offset retain literal or parameter operands and their locations. Except for LIMIT 0
and nonblocking expression/derived/joined sources, paging runs last,
after grouping, DISTINCT and ordering. Planning validates counts even for direct AST requests and
EXPLAIN: invalid kinds, NULL, negative, fractional or out-of-Int64 values raise DatabaseException
with CD00007 at the operand; unbound parameters use CD01005. No SQLSTATE is assigned.
See [LIMIT/OFFSET](../../../../database/document/language/clauses/limit-offset.md).

Construction stores arguments and does not execute. Engine validation rejects an empty projection,
or IsValue with multiple expressions, an alias or unqualified star, with DatabaseException `CD05001`.
Under IsDistinct, an ORDER BY expression outside the projection after alias rewriting raises
`CD01014`. Duplicate projected names use `CD01003`. Inspect parser diagnostics before planning.

Bindings contains ordered UNNEST/JOIN/NEST bindings. A non-null bindings argument takes precedence
over unnest; Unnest then exposes only its array bindings. With no key bindings, Bindings uses the
ordinary Unnest representation. Construction retains children without executing. Expansion precedes Predicate. A direct SELECT AST with a repeated binding
or empty UNNEST name raises DatabaseException/CD01023. Parser duplicates use CD00010, including
collision with an unaliased collection name. Star with bindings constructs synthetic objects.
Collection and inferred UNNEST names are valid implicit aliases, including in joined statements.
CD00016 is retired and reserved. See [OqlJoinExpression](oql-join-expression.md),
[OqlUnnestExpression](oql-unnest-expression.md) and [UNNEST](../../../../database/document/language/clauses/unnest.md).

GroupAliases is empty when no aliases are supplied, or parallel to GroupBy with null entries for
unnamed terms. GroupAs is the optional group-array name. Construction stores metadata without
validation. Planning raises DatabaseException/CD01030 for missing grouping keys, a nonempty alias
list of the wrong width, empty names or collisions with group/FROM/UNNEST names. CD01031 rejects
the group array in an aggregate row input; parser CD00012 rejects GROUP AS without GROUP BY.
None has an assigned SQLSTATE. See [GROUP BY](../../../../database/document/language/clauses/group-by.md).

Each OrderBy entry is `OqlOrdering(OqlExpression Expression, bool Descending,
OqlNullPlacement NullPlacement = OqlNullPlacement.Default)`. [OqlNullPlacement](oql-null-placement.md)
retains the written suffix; invalid enum values fail planning with DatabaseException/CD01032
(no SQLSTATE). Text malformed suffixes use CD00013. See [ORDER BY](../../../../database/document/language/clauses/order-by.md).

Source is null for a collection source. Supplying the optional source constructor argument
ignores collection; Collection then returns an empty string. A derived source is an
[OqlSubqueryExpression](oql-subquery-expression.md); other sources retain their expression node.
Construction only stores these arguments. Planning raises DatabaseException/CD01033 for a missing
source alias, CD01034 for an unbound source variable. Correlated sources evaluate per outer row. Text without a
derived alias reports CD00015. None has an assigned SQLSTATE. LIMIT 0 validates statically then
returns planned metadata without executing. See [FROM](../../../../database/document/language/clauses/from.md).

Alias retains the supplied iteration name, including an inferred expression-source name. It may
be null for an unresolved implicit collection binding; planner copies resolve the name and keep
IsAliasImplicit true. The constructor sets IsAliasImplicit when alias is null or isAliasImplicit
is true. A stored collection uses its whole name; a system source uses its last segment as written.
Only explicit aliases allow an inner bare FROM name to correlate. Inferred names still work in
qualified paths, grouping, ordering, index analysis and EXPLAIN.

Windows retains query-local named [OqlWindowDefinition](oql-window-definition.md) declarations,
or an empty list. The optional windows argument follows isAliasImplicit; a null member throws
ArgumentNullException. Named definitions precede final ordering and paging. Empty/duplicate names
use CD01041; unknown/self/forward references CD01042; invalid copies/OVER shapes CD01043.
Windows execute after HAVING and before projection/DISTINCT/final ordering. See
[window defaults and placement](../../../../database/document/language/statements/select.md#windows).

## Example

Adapted from OqlGroupingParserTests:

```csharp
var grouped = (OqlSelectExpression)((OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT key, g FROM items GROUP BY k AS key GROUP AS g")).OqlExpression;
Console.WriteLine(grouped.GroupAliases[0]); // key
Console.WriteLine(grouped.GroupAs);         // g
```

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT DISTINCT RAW x FROM items ORDER BY x");
var select = (OqlSelectExpression)statement.OqlExpression;
Console.WriteLine(select.IsDistinct); // True
Console.WriteLine(select.IsValue);    // True
```

For documents `{"x":null}`, `{}`, `{}`, execution returns one bare NULL row; IsMissing(0)
is false and IsNull(0) is true in process and through the client.

```csharp
var page = (OqlSelectExpression)((OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT * FROM items LIMIT @take OFFSET +1")).OqlExpression;
Console.WriteLine(page.Limit is OqlParameterExpression); // True
Console.WriteLine(page.Offset is not null);             // True
```

With take = 1, paging returns the second row in the baseline document-ID order.

From OqlSourceParserTests:

```csharp
var derived = (OqlSelectExpression)((OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT * FROM (SELECT n FROM items) d")).OqlExpression;
Console.WriteLine(derived.Collection);                      // empty string
Console.WriteLine(derived.Source is OqlSubqueryExpression); // True
Console.WriteLine(derived.Alias);                           // d
Console.WriteLine(derived.IsAliasImplicit);                 // False
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryExpression.Select.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlProjectionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentProjectionWireTests.cs`.
