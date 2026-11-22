# CREATE INDEX

Creates a named, nonunique index on one node property for a label.

> **Status:** Implemented. Single-property node indexes only.

## Syntax

```syntaxsql
CREATE [ TEXT ] INDEX [ index_name ] [ IF NOT EXISTS ] FOR ( variable : label )
ON ( variable . property )
```

## Arguments

- **`index_name`** — Optional database-wide, case-sensitive index name.
- **`label`** — The node label to index.
- **`property`** — One property key, referenced through the same variable declared by FOR.
- **`IF NOT EXISTS`** — Skips an existing index with that name or an equivalent index on the label/property.

## Remarks

Creation discovers an unseen label and untyped property key, builds over existing nodes and
publishes the catalog and physical index atomically. The index adds no required-property or
type rule: use a constraint to require a non-NULL property. It creates no graph entity. Failure
rolls back the index and every newly discovered token. Dropping the index retains its tokens.
UNIQUE, composite and relationship-property indexes are refused with CK00001.

### Names and conditional creation

Index and constraint names share one database-wide namespace with ordinal, case-sensitive
identity. IF NOT EXISTS suppresses a same-kind name or equivalent owner/property/type rule,
even under another name. Exact and text indexes are distinct kinds for pair equivalence, and
may coexist once per kind and label/property. IF NOT EXISTS suppresses an existing index name
of either kind. A collision with a constraint name still uses CG03008; incompatible
constraint rules use CG03009. A real create/drop affects 1; a conditional no-op affects 0.

### Generated names

Omitting the name generates `index_<sha256>` or, for a constraint, `constraint_<sha256>`,
using all 64 lowercase hexadecimal digest characters. Identity depends on object kind,
node/relationship owner kind, owner name, property and canonical type (or existence).
Variable names, aliases, whitespace and IF NOT EXISTS do not change it. Read the generated
name through [SHOW INDEXES or SHOW CONSTRAINTS](show.md) before a named drop.

### Transactions and limits

Schema DDL is standalone on the session database, with no parameter operands, query
composition or EXPLAIN. Submit each statement separately. Text and typed schema writes
run in one autocommit transaction, including discovery, validation and publication.
Cohesion refuses every schema write inside an explicit transaction with CG04025, including
conditional no-ops. The refusal aborts that transaction; ROLLBACK restores session use.
This is a Cohesion policy, also applied by the [GraphSchema API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-schema.md).

DDL observes statement work and retained-memory limits. Text requests can lower them with
GraphQueryLimits; typed writes use engine ceilings. Owner scans and backfill charge work;
cursor buffers and retained backfill entries charge memory. Exhaustion rolls back the
whole operation, including new tokens and name reservations.

## Text indexes

`CREATE TEXT INDEX [name] [IF NOT EXISTS] FOR (n:Label) ON (n.property)` builds a nonunique
index over Unicode code-point trigrams, without case folding or normalization. It shares exact
indexes' node-only, single-property grammar, autocommit and naming rules. Generated names
distinguish text indexes. Use DROP INDEX name for either kind; DROP TEXT INDEX remains unsupported.
[SHOW INDEXES](show.md) keeps the same columns for both kinds; typed GraphIndexMetadata.IsText
identifies the kind.

A direct property `=`, STARTS WITH, ENDS WITH or CONTAINS on a required label in a MATCH AND
chain can use the index; inline property equalities and MERGE share selection. It reads **one
anchor: the pattern's first trigram**, with no posting intersection. Selectivity is only that
of this trigram. Every candidate receives the stored-value, label and identity recheck, so
qualifying rows equal a label scan, preserving predicate position and multiplicity. Comparisons
remain ordinal and case-sensitive.

Patterns under three code points, including empty strings, use a plain label scan without a
prefilter on stored or overlay rows. A supplementary pair counts as one code point; combining
marks count separately. Parameters bind anew for every execution, with eligibility rechecked
when the cursor opens. EXPLAIN reports TextIndexSeek or LabelScan, the parameter/execution
decision, first-trigram anchor and mandatory stored-value recheck. NULL and non-string patterns
cannot supply text seeks.

A text index **never moves a plan's anchor**: after exact seeks and relationship anchors compete,
it can only replace the label scan on the already selected node and label. It cannot take
another node's tie. Estimates use label cardinality alone. Each visible anchor posting costs
one work unit; extracting that anchor charges no work and retains no directory for the rest
of the pattern. A real seek can avoid residual errors on excluded rows, as exact seeks can;
short-pattern scans evaluate the ordinary full predicate.

### Write costs and capacity

A stored string of N code points contributes at most N-2 distinct postings, and strings under
three contribute none. Each version change tombstones old tokens and inserts new tokens.
For L code points and K distinct trigrams, build costs 1+L+K work units per node with a string
(label visit, extraction, insertion). SET costs one label visit plus L_old+L_new per maintained
text index. An unrelated `SET n.seen=true` pays the same extraction/token maintenance because
the stored version moves. Extraction reserves 128 bytes plus 256 per distinct token; staged
rows, keys and backfill buffers add charges. Defaults remain 1,000,000 work units and 256 MiB.

With one initial p property per node, adjacent first-refusal boundaries at default grants are:

| Statement / shape | Text first refused row | Exact first refused row |
| --- | --- | --- |
| SET p from vwxyz to abcde | 82,545 (CG05020) | 141,133 (CG05020) |
| Unrelated SET, 40 code points / 38 distinct trigrams | 12,346 (CG05004) | 130,817 (CG05020) |
| Index build, 20 code points / 18 distinct trigrams | 25,642 (CG05004) | 500,001 (CG05004) |

These posting costs are an exception to the general 100,000-row envelope. Width, repeated
trigrams and other retained state change capacity. CG05004/CG05020 failure is atomic. Build
insertion work is admitted before allocating index pages. Properties and nodes still must
fit the single-record limit (CG02009); a 1 MiB string writes no text postings.

CREATE TEXT INDEX and DROP INDEX wait for journal admission outside catalog/directory monitors;
metadata and data reads remain available. A backfill already admitted when J/2 is crossed
finishes its physical bracket and mandatory commit, and can overshoot J.

Catalog v4 admits the new kind; v3 refuses CG06019 before writes with all files unchanged.
Graph entity format remains 5, and current B-tree pages are format 3.

## Diagnostics

These conditions have no assigned SQLSTATE:

| Code | Meaning and trigger |
| --- | --- |
| CK00001 | UNIQUE, composite or relationship-property index form |
| CG01022 | ON references a variable other than the FOR declaration |
| CG03008 | Index/constraint name is already used |
| CG03003 | Another index of the same kind already targets the label/property without a conditional no-op |
| CG04025 | Schema write inside an explicit transaction; transaction is aborted |
| CG05004 | DDL work budget exhausted |
| CG05020 | DDL retained-memory budget exhausted |
| CG08021 | Invalid GraphQueryLimits, validated inside the admitted operation |

Schema ownership protects definitions and their children. See [Diagnostics](../diagnostics.md)
for ownership, cancellation, oversized metadata, offline and commit failures.

## Example

Execute separately in a fresh database:

```sql
CREATE INDEX by_name FOR (n:Person) ON (n.name)
CREATE INDEX by_name IF NOT EXISTS FOR (n:Person) ON (n.name)
SHOW INDEXES
```

Affected counts are 1 then 0. SHOW returns one row: the session database, Person's generated
label GUID, `Person`, `by_name`, `name`, `FALSE`. No Person node is inserted.

## See also

[DROP INDEX](drop-index.md) · [CREATE CONSTRAINT](create-constraint.md) · [Statements](index.md)

## Sources

- **Text indexes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`, `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphTextIndexTests.cs` and its partials.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaDdlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.Ddl.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`.
