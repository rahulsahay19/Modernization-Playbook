# Business Capabilities

## Capability Map

| Capability | Business responsibility | Current implementation | Classification |
|---|---|---|---|
| Claims Processing | Intake, validation, adjudication, and claim lifecycle | `ClaimsWorkflowService`, `Claim`, `ClaimLine` | Core |
| Policy Management | Define products, limits, deductibles, and policy availability | `PolicyService`, `Policy` | Supporting |
| Member Management | Maintain covered members and policy enrollment | `MemberService`, `Member` | Supporting |
| Provider Network | Maintain eligible healthcare providers and network tiers | `ProviderService`, `Provider` | Supporting |
| Document Management | Register and verify claim evidence | `DocumentService`, `ClaimDocument` | Supporting |
| Payment And Settlement | Schedule and settle approved claim payments | `PaymentService`, `Payment` | Supporting |
| Member Communications | Deliver email and SMS notifications | `NotificationService`, `NotificationMessage` | Generic |
| Audit | Record accountable business actions | `AuditService`, `AuditEntry` | Generic |
| Operational Reporting | Provide cross-capability operational views | `ReportingService` | Analytical |

## Core Domain

### Claims Processing

Claims Processing is the core domain because it contains the business decisions
that turn healthcare services and policy coverage into an approved, rejected,
or paid claim.

Current rules include:

- the member must exist and be active;
- the member must have an active policy;
- the provider must exist and be active;
- the requested amount cannot exceed the policy annual limit;
- all uploaded documents must be verified before approval;
- only approved claims can be scheduled for payment.

The current model is intentionally simple. Future discovery can introduce richer
concepts such as coverage evaluation, benefit utilization, exclusions,
preauthorization, fraud review, and adjudication reasons.

## Supporting Domains

### Policy Management

Owns insurance products and financial constraints. It supplies coverage facts
to Claims Processing but should not own claim decisions.

### Member Management

Owns the covered person's identity, contact details, status, and enrollment
reference. It supplies eligibility facts to Claims Processing.

### Provider Network

Owns provider participation, location, and network tier. It supplies provider
eligibility and network facts to Claims Processing.

### Document Management

Owns evidence metadata, storage location, document type, and verification
outcome. It should communicate document state without directly changing the
claim aggregate.

### Payment And Settlement

Owns disbursement scheduling, payment mode, settlement reference, failure
reason, and settlement date. It should communicate settlement outcomes without
directly changing the claim aggregate.

## Generic And Analytical Capabilities

### Member Communications

Email and SMS delivery are reusable technical capabilities. Business contexts
should request a communication using a template or business intent instead of
constructing and persisting messages as part of their own transaction.

### Audit

Audit records actions across contexts. It is important for traceability but does
not make core claim decisions.

### Operational Reporting

Reporting combines facts from multiple contexts. It should eventually consume
published data or projections rather than read every operational schema.

## Capability Dependency Matrix

| Consumer | Policy | Member | Provider | Claims | Documents | Payments | Notifications | Audit |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Claims Processing | Read | Read | Read | Own | Read | - | Write | Write |
| Member Management | Read | Own | - | - | - | - | - | Write |
| Document Management | - | Read | - | Write | Own | - | Write | Write |
| Payment And Settlement | - | Read | - | Write | - | Own | Write | Write |
| Reporting | Read | Read | Read | Read | Read | Read | Read | - |

The `Write` relationships into Claims are the most important ownership
violations to remove during modularization.

## Recommended Ubiquitous Language

| Term | Meaning |
|---|---|
| Claim | A request for reimbursement for healthcare services |
| Claim line | One billed service or charge within a claim |
| Adjudication | Evaluation resulting in approval or rejection |
| Eligibility | Whether a member, policy, or provider is valid for processing |
| Evidence | A document supporting a claim |
| Document verification | Confirmation that submitted evidence is acceptable |
| Approved amount | Amount authorized for payment |
| Settlement | Completion of the financial transfer |
| Network tier | Provider participation classification |

The code and course narrative should use these terms consistently before new
technical boundaries are introduced.
