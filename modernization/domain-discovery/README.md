# Domain Discovery

This folder records the architectural reasoning used before changing the
healthcare claims monolith. The conclusions are based on the behavior and
dependencies visible in the existing application, not on a desired
microservices diagram.

## Discovery Sequence

1. [Current-state assessment](01-current-state-assessment.md)
2. [Business capabilities](02-business-capabilities.md)
3. [Domain events](03-domain-events.md)
4. [Bounded contexts](04-bounded-contexts.md)
5. [Context map](05-context-map.md)
6. [Extraction priorities](06-extraction-priorities.md)

## Branch Outcome

This branch produces a proposed domain model and modernization direction. It
does not restructure the monolith, create modules, introduce messaging, or
extract services. Those implementation decisions follow only after the proposed
boundaries have been reviewed.

## Working Assumptions

- Claims adjudication is the core differentiating business capability.
- Policy, member, and provider data are authoritative reference capabilities
  consumed during claim processing.
- Documents and payments have independent workflows and are strong candidates
  for clearer ownership.
- Notifications and audit are cross-cutting capabilities.
- Reporting is a downstream consumer of operational data, not an owner of
  transactional state.
- The React portal remains a stable client of API contracts throughout the
  modernization journey.
