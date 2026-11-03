# OqlJoinExpression

A scalar document-key JOIN or NEST binding.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlJoinExpression : OqlSourceBindingExpression
{
    public OqlJoinExpression(string collection, string? variable, OqlExpression key,
        bool isLeft = false, bool isNest = false, Location? location = null);
    public string Collection { get; }
    public string? Alias { get; }
    public bool IsNest { get; }
}
```

## Behavior and exceptions

Collection names the stored right-hand collection. Inherited Expression retains key, Variable is
the resolved binding name and IsLeft preserves unmatched inputs. Alias is the explicit declaration,
or null when the constructor variable is null and the collection supplies the implicit name. IsNest selects arrays: matched keys
bind one-element arrays, absent strings bind `[]` under LEFT NEST, and NULL/MISSING keys bind
MISSING. JOIN instead binds a document or MISSING under LEFT JOIN.

Construction retains arguments without execution or validation. Implicit names participate in
duplicate checks (CD00010 in text, CD01023 in direct ASTs). Only an explicit alias permits an
inner bare FROM name to correlate; qualified paths may capture either kind of binding.
Planning uses CD01035 for self/forward references and invalid joined META. Non-string,
non-NULL/MISSING keys raise CD02028. Shared retained-memory/work exhaustion uses CD05018/CD05020.
These DatabaseException diagnostics have no assigned SQLSTATE. See
[JOIN and NEST](../../../../database/document/language/clauses/join.md) for scope, stars and cache limits.

## Example

Adapted from OqlKeyJoinParserTests:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT b.* FROM items AS a LEFT NEST docs AS b ON META(b).id = a.key");
var select = (OqlSelectExpression)statement.OqlExpression;
var join = (OqlJoinExpression)select.Bindings[0];
Console.WriteLine(join.Collection); // docs
Console.WriteLine(join.Variable);   // b
Console.WriteLine(join.Alias);      // b
Console.WriteLine(join.IsLeft);     // True
Console.WriteLine(join.IsNest);     // True
```

## Related reference

[OqlSourceBindingExpression](oql-source-binding-expression.md) · [OqlSelectExpression](oql-select-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlJoinExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlKeyJoinParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentKeyJoinTests.cs`.
