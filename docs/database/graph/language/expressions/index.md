# Expressions

GQL expressions provide scalar computation, Boolean predicates, parameters and bound references.

> **Status:** Implemented.

- **[Operators](operators.md)** — Boolean, arithmetic, null/truth, membership and string predicates.
- **[Parameters](parameters.md)** — runtime scalar, list and map bindings in maps, WHERE and RETURN.
- **[Literals](literals.md)** — null, Boolean, integer, floating-point, and string values.

- **[CAST](cast.md)** — SQL-compatible scalar conversion and its difference from Cypher toX calls.
- **[CASE](case.md)** — searched/simple forms, ISO when-operand lists and lazy result selection.
- **[COALESCE](coalesce.md)** — first non-NULL value, evaluated left to right.
- **[NULLIF](nullif.md)** — NULL for TRUE equality, otherwise the first value.
- **[List values](list-values.md)** — ordered constructors, membership, comparison and value limits.
- **[List comprehensions](list-comprehensions.md)** — scoped filtering and projection.
- **[reduce](reduce.md)** — ordered folding with local accumulator and iterator.
- **[List predicates](list-predicates.md)** — all/any/none/single with three-valued results.
- **[List slices](list-slices.md)** — inclusive/exclusive bounds and negative offsets.
- **[Map values](map-values.md)** — ordinal named entries with nested values.
- **[Subscripts and property access](property-access.md)** — list indexes, map fields and entity properties.

WHERE and RETURN accept these value expressions. [Aggregates](../functions/aggregate-functions.md)
execute in RETURN and use dedicated aggregate nodes; COALESCE/NULLIF are case abbreviations.
[Scalar functions](../functions/scalar-functions.md) and [CAST](cast.md) also execute with plan-time
signature checks and strict NULL propagation.

## See also

- **[Language (GQL)](../index.md)** — reference hub.
- **[WHERE](../clauses/where.md)** — predicate composition.
- **[RETURN](../clauses/return.md)** — projection grammar.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Expressions.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
