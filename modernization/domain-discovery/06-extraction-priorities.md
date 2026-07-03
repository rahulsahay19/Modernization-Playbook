# Extraction Priorities

## Do Not Start With Microservices

The immediate next architecture should be a modular monolith. This allows the
team to validate boundaries and contracts without simultaneously introducing:

- distributed transactions;
- network failure;
- message delivery guarantees;
- independent deployment pipelines;
- service discovery;
- distributed observability;
- production data migration.

Extraction becomes safer after module ownership is enforced in-process.

## Evaluation Criteria

Each candidate is scored from 1 to 5.

| Criterion | Meaning |
|---|---|
| Business value | Benefit from independent evolution |
| Boundary clarity | How clearly its data and behavior can be isolated |
| Coupling | Current dependency on other capabilities; lower is easier |
| Operational need | Benefit from independent scaling or resilience |
| Migration risk | Data and workflow risk; lower is safer |

## Candidate Assessment

| Candidate | Business value | Boundary clarity | Current coupling | Operational need | Migration risk | Interpretation |
|---|---:|---:|---:|---:|---:|---|
| Documents | 4 | 5 | 3 | 4 | 2 | Strong first extraction candidate |
| Communications | 3 | 5 | 2 | 4 | 2 | Easy boundary; useful early event-driven example |
| Reporting | 4 | 4 | 5 | 4 | 3 | Valuable but needs projection strategy |
| Payments | 5 | 4 | 4 | 4 | 5 | High value and high financial risk |
| Provider Network | 3 | 4 | 2 | 2 | 2 | Clear reference-data boundary |
| Policy | 5 | 4 | 4 | 3 | 4 | Central eligibility dependency |
| Membership | 5 | 4 | 4 | 3 | 4 | Central identity and eligibility dependency |
| Claims | 5 | 3 | 5 | 4 | 5 | Core domain; extract after surrounding boundaries stabilize |

## Recommended Modernization Sequence

### Phase 1: Modular Monolith

Create enforced modules for:

1. Claims
2. Policy
3. Membership
4. Provider Network
5. Documents
6. Payments
7. Communications
8. Audit
9. Reporting

Each module receives its own domain model, application layer, infrastructure
area, and public contract. The first database may remain physically shared, but
modules must not access one another's repositories or entity classes.

### Phase 2: Introduce An API Gateway

Place a stable routing layer in front of the backend before extraction. The
React portal calls the gateway, and the gateway initially routes everything to
the monolith.

### Phase 3: Extract Documents

Documents is the recommended first business-service extraction because:

- ownership is clear;
- document workloads may scale independently;
- storage can be isolated;
- the existing workflow already identifies it as a separate capability;
- temporary inconsistency is manageable;
- it demonstrates the strangler pattern without moving core adjudication first.

Required preparation:

- stop direct claim updates from `DocumentService`;
- define document commands and evidence-status contracts;
- create a document-owned schema;
- introduce an outbox for `ClaimDocumentUploaded`,
  `ClaimDocumentVerified`, and `ClaimDocumentRejected`;
- migrate document metadata and storage references;
- route `/api/documents` through the gateway.

### Phase 4: Extract Communications

Move email and SMS delivery behind asynchronous communication intents. This
demonstrates resilience, retries, and dead-letter handling without risking claim
state.

### Phase 5: Extract Reporting

Build reporting projections from published facts. Stop cross-schema operational
queries before databases are physically separated.

### Phase 6: Extract Payments

Extract only after:

- approved-claim contracts are stable;
- idempotency is implemented;
- settlement reconciliation exists;
- outbox/inbox patterns are proven;
- operational monitoring and alerting are available.

### Phase 7: Evaluate Reference And Core Domains

Provider Network may be extracted before Policy and Membership because it has
fewer dependencies. Policy and Membership should follow only when their API
contracts and source-of-truth decisions are clear.

Claims should not automatically become a microservice. Keep it modular unless
independent scaling, ownership, release cadence, or organizational needs justify
extraction.

## Why Claims Is Not First

Claims is the highest-value domain but also the most connected. Extracting it
first would require immediate remote dependencies on policy, membership,
providers, documents, payments, notifications, audit, and reporting. That would
recreate the monolith as a distributed monolith.

## Branch Decision

The next implementation branch should create module boundaries inside a new
modular-monolith solution. The first technical goal is not service extraction;
it is preventing one capability from reaching into another capability's
repositories and data model.
