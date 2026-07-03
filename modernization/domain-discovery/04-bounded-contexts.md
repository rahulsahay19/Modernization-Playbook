# Proposed Bounded Contexts

## Boundary Principles

The proposed contexts follow business language and data ownership. They are not
one-to-one copies of controllers or database tables.

Each context should eventually:

- own its domain model and invariants;
- expose explicit commands, queries, or events;
- prevent another context from directly updating its state;
- control its persistence model;
- evolve without sharing internal entity classes.

## Contexts

### Claims

**Classification:** Core

**Owns:**

- claim identity;
- claim lines;
- requested and approved amounts;
- claim lifecycle;
- adjudication decisions and reasons;
- evidence requirements as understood by claims.

**Consumes:**

- member eligibility;
- policy coverage;
- provider eligibility;
- document verification status;
- payment settlement outcome.

**Does not own:**

- member profile;
- policy product definition;
- provider directory;
- binary documents;
- bank settlement details;
- message delivery.

### Policy

**Classification:** Supporting

**Owns:**

- policy number;
- insurance provider;
- plan;
- limits and deductibles;
- policy status;
- coverage rules as the domain becomes richer.

**Publishes:**

- `PolicyCreated`;
- `PolicyUpdated`;
- `PolicyDeactivated`.

### Membership

**Classification:** Supporting

**Owns:**

- member identity;
- personal and contact information;
- enrollment;
- member activation status.

The current `Member.PolicyNumber` foreign key becomes an enrollment reference or
policy identifier contract rather than a cross-context entity relationship.

### Provider Network

**Classification:** Supporting

**Owns:**

- provider identity;
- provider name;
- location;
- network tier;
- participation status.

### Documents

**Classification:** Supporting

**Owns:**

- document identity;
- storage reference;
- document type;
- verification status;
- rejection reason;
- document-processing metadata.

Documents should report evidence facts to Claims. It should not directly update
the claim lifecycle.

### Payments

**Classification:** Supporting

**Owns:**

- payment identity;
- claim reference;
- amount;
- payment mode;
- schedule;
- settlement reference;
- settlement and failure status.

Payments accepts an approved claim for payment and reports settlement outcomes.
It should not directly update the claim aggregate.

### Communications

**Classification:** Generic

**Owns:**

- notification request;
- channel;
- delivery status;
- provider response;
- retries and failure details.

Business contexts supply communication intent and recipient references.

### Audit

**Classification:** Generic

**Owns:**

- immutable action records;
- actor;
- timestamp;
- entity or aggregate reference;
- compliance metadata.

Audit is conceptually distinct from diagnostics logging.

### Reporting

**Classification:** Analytical

**Owns:**

- operational projections;
- aggregated measures;
- query-optimized read models.

Reporting should not own or update transactional facts.

## Aggregate Candidates

| Context | Aggregate root | Important invariants |
|---|---|---|
| Claims | Claim | Valid lifecycle transitions; approved amount rules; evidence requirement before approval |
| Policy | Policy | Unique policy number; valid monetary limits; controlled activation |
| Membership | Member | Unique member ID; valid enrollment reference; controlled activation |
| Provider Network | Provider | Unique provider ID; valid network status |
| Documents | ClaimDocument | Valid verification transitions; rejection reason when rejected |
| Payments | Payment | Approved amount is immutable after scheduling; valid settlement transitions |
| Communications | Notification | Valid delivery lifecycle and retry behavior |
| Audit | Audit Record | Append-only and immutable |

## Ownership Corrections

| Current behavior | Proposed ownership |
|---|---|
| Document upload directly sets `Claim.Status` | Documents publishes evidence state; Claims decides its transition |
| Payment settlement directly sets `Claim.Status` | Payments publishes settlement; Claims marks itself paid |
| Claims constructs notification records | Claims emits intent/event; Communications owns delivery |
| Every service appends audit entity directly | Audit handles business-event or audit-contract messages |
| Reporting repository reads all tables | Reporting builds projections from context-owned data |

## Boundary Decision

These are logical boundaries first. They should initially become modules in a
modular monolith. Deployment boundaries can be chosen later using change rate,
scaling, team ownership, operational risk, and extraction cost.
