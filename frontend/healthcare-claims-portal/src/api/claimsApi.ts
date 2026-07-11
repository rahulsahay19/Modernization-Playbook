import {
  demoAudit,
  demoClaims,
  demoDashboard,
  demoDocuments,
  demoMembers,
  demoNotifications,
  demoPayments,
  demoPolicies,
  demoProviders,
} from './demoData'
import type {
  AuditEntry,
  Claim,
  ClaimDocument,
  Dashboard,
  Member,
  NotificationMessage,
  Payment,
  Policy,
  Provider,
  StatusCount,
} from './types'

const baseUrl = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '')

type ApiRecord = Record<string, unknown>

async function get<T>(path: string): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    headers: { Accept: 'application/json' },
  })

  if (!response.ok) {
    throw new Error(`API request failed with ${response.status}`)
  }

  return response.json() as Promise<T>
}

function asArray(value: unknown): ApiRecord[] {
  return Array.isArray(value) ? (value as ApiRecord[]) : []
}

function asRecord(value: unknown): ApiRecord {
  return value && typeof value === 'object' ? (value as ApiRecord) : {}
}

function text(value: unknown, fallback = ''): string {
  return typeof value === 'string' ? value : fallback
}

function number(value: unknown, fallback = 0): number {
  return typeof value === 'number' ? value : fallback
}

function bool(value: unknown, fallback = false): boolean {
  return typeof value === 'boolean' ? value : fallback
}

function isActiveStatus(value: unknown): boolean {
  const normalized = text(value).toLowerCase()
  return normalized === 'active' || normalized === 'graceperiod' || normalized === 'preferred'
}

function statusCounts(items: { status: string }[]): { status: string; count: number }[] {
  const counts = new Map<string, number>()

  for (const item of items) {
    counts.set(item.status, (counts.get(item.status) ?? 0) + 1)
  }

  return [...counts.entries()].map(([status, count]) => ({ status, count }))
}

function normalizePolicies(value: unknown): Policy[] {
  return asArray(value).map((item) => ({
    policyNumber: text(item.policyNumber),
    insuranceProviderName: text(item.insuranceProviderName, text(item.providerName)),
    planName: text(item.planName, text(item.productName)),
    annualLimit: number(item.annualLimit, number(item.sumInsured)),
    deductible: number(item.deductible),
    isActive: typeof item.isActive === 'boolean' ? bool(item.isActive) : isActiveStatus(item.status),
  }))
}

function normalizeMembers(value: unknown): Member[] {
  return asArray(value).map((item) => {
    const memberId = text(item.memberId, text(item.memberNumber))

    return {
      memberId,
      fullName: text(item.fullName),
      policyNumber: text(item.policyNumber),
      dateOfBirth: text(item.dateOfBirth),
      email: text(item.email, `${memberId.toLowerCase()}@example.local`),
      mobileNumber: text(item.mobileNumber),
      isActive: typeof item.isActive === 'boolean' ? bool(item.isActive) : isActiveStatus(item.status),
    }
  })
}

function normalizeProviders(value: unknown): Provider[] {
  return asArray(value).map((item) => ({
    providerId: text(item.providerId, text(item.providerCode)),
    name: text(item.name),
    networkTier: text(item.networkTier),
    city: text(item.city),
    isActive: typeof item.isActive === 'boolean' ? bool(item.isActive) : isActiveStatus(item.status),
  }))
}

function normalizeClaims(value: unknown): Claim[] {
  return asArray(value).map((item) => {
    const lines = asArray(item.lines).map((line, index) => ({
      code: text(line.code, `LINE-${index + 1}`),
      description: text(line.description),
      amount: number(line.amount),
    }))
    const requestedAmount = number(item.requestedAmount, number(item.totalAmount))

    return {
      claimNumber: text(item.claimNumber),
      memberId: text(item.memberId, text(item.memberNumber)),
      providerId: text(item.providerId, text(item.providerCode)),
      serviceDate: text(item.serviceDate, text(item.dateOfService)),
      requestedAmount,
      approvedAmount: number(item.approvedAmount),
      status: text(item.status),
      lines,
      documentIds: Array.isArray(item.documentIds) ? (item.documentIds as string[]) : [],
      createdAt: text(item.createdAt, text(item.submittedOn)),
    }
  })
}

function normalizeDocuments(value: unknown): ClaimDocument[] {
  return asArray(value).map((item) => ({
    documentId: text(item.documentId, text(item.id)),
    claimNumber: text(item.claimNumber),
    fileName: text(item.fileName),
    documentType: text(item.documentType),
    storagePath: text(item.storagePath, text(item.storageReference)),
    status: text(item.status),
    rejectionReason: text(item.rejectionReason, text(item.notes)) || null,
    uploadedAt: text(item.uploadedAt, text(item.receivedOn)),
  }))
}

function normalizePayments(value: unknown): Payment[] {
  return asArray(value).map((item) => ({
    paymentId: text(item.paymentId, text(item.paymentNumber, text(item.id))),
    claimNumber: text(item.claimNumber),
    amount: number(item.amount),
    status: text(item.status),
    paymentMode: text(item.paymentMode, 'NEFT'),
    settlementReference: text(item.settlementReference, text(item.remarks)) || null,
    failureReason: text(item.failureReason) || null,
    scheduledDate: text(item.scheduledDate, text(item.scheduledOn, text(item.createdOn))),
    settledDate: text(item.settledDate) || null,
  }))
}

function normalizeNotifications(value: unknown): NotificationMessage[] {
  return asArray(value).map((item) => ({
    notificationId: text(item.notificationId, text(item.id)),
    channel: text(item.channel),
    status: text(item.status),
    recipient: text(item.recipient),
    subject: text(item.subject),
    body: text(item.body),
    createdAt: text(item.createdAt, text(item.createdOn)),
    sentAt: text(item.sentAt) || null,
  }))
}

function normalizeAudit(value: unknown): AuditEntry[] {
  return asArray(value).map((item) => ({
    auditId: text(item.auditId, text(item.id)),
    entityType: text(item.entityType, text(item.module)),
    entityId: text(item.entityId, text(item.entityReference)),
    action: text(item.action, text(item.eventName)),
    description: text(item.description, text(item.summary)),
    performedBy: text(item.performedBy, 'system'),
    performedAt: text(item.performedAt, text(item.occurredOn)),
  }))
}

function normalizeDashboard(
  value: unknown,
  policies: Policy[],
  members: Member[],
  providers: Provider[],
  claims: Claim[],
  documents: ClaimDocument[],
  payments: Payment[],
  notifications: NotificationMessage[],
): Dashboard {
  const report = asRecord(value)
  const requestedAmount = number(
    report.requestedAmount,
    number(report.submittedClaimAmount, claims.reduce((sum, claim) => sum + claim.requestedAmount, 0)),
  )
  const approvedAmount = number(
    report.approvedAmount,
    number(report.approvedClaimAmount, claims.reduce((sum, claim) => sum + claim.approvedAmount, 0)),
  )
  const settledAmount = number(
    report.settledAmount,
    number(report.settledPaymentAmount, payments.reduce((sum, payment) => sum + payment.amount, 0)),
  )

  return {
    totalPolicies: number(report.totalPolicies, policies.length),
    totalMembers: number(report.totalMembers, members.length),
    totalProviders: number(report.totalProviders, providers.length),
    totalClaims: number(report.totalClaims, claims.length),
    claimsByStatus: Array.isArray(report.claimsByStatus)
      ? (report.claimsByStatus as StatusCount[])
      : statusCounts(claims),
    documentsByStatus: Array.isArray(report.documentsByStatus)
      ? (report.documentsByStatus as StatusCount[])
      : statusCounts(documents),
    paymentsByStatus: Array.isArray(report.paymentsByStatus)
      ? (report.paymentsByStatus as StatusCount[])
      : statusCounts(payments),
    requestedAmount,
    approvedAmount,
    settledAmount,
    pendingDocuments: number(
      report.pendingDocuments,
      documents.filter((document) => document.status !== 'Verified').length,
    ),
    uploadedDocuments: number(report.uploadedDocuments, documents.length),
    scheduledPayments: number(
      report.scheduledPayments,
      payments.filter((payment) => payment.status === 'Scheduled').length,
    ),
    notificationsSent: number(
      report.notificationsSent,
      notifications.filter((notification) => notification.status === 'Sent').length,
    ),
  }
}

export const claimsApi = {
  async loadPortal() {
    try {
      const [
        dashboard,
        claims,
        members,
        providers,
        policies,
        documents,
        payments,
        notifications,
        audit,
      ] = await Promise.all([
        get<unknown>('/api/reports/operations'),
        get<unknown>('/api/claims'),
        get<unknown>('/api/members'),
        get<unknown>('/api/providers'),
        get<unknown>('/api/policies'),
        get<unknown>('/api/documents'),
        get<unknown>('/api/payments'),
        get<unknown>('/api/notifications'),
        get<unknown>('/api/audit'),
      ])
      const normalizedClaims = normalizeClaims(claims)
      const normalizedMembers = normalizeMembers(members)
      const normalizedProviders = normalizeProviders(providers)
      const normalizedPolicies = normalizePolicies(policies)
      const normalizedDocuments = normalizeDocuments(documents)
      const normalizedPayments = normalizePayments(payments)
      const normalizedNotifications = normalizeNotifications(notifications)
      const normalizedAudit = normalizeAudit(audit)
      const normalizedDashboard = normalizeDashboard(
        dashboard,
        normalizedPolicies,
        normalizedMembers,
        normalizedProviders,
        normalizedClaims,
        normalizedDocuments,
        normalizedPayments,
        normalizedNotifications,
      )

      return {
        usingDemoData: false,
        data: {
          dashboard: normalizedDashboard,
          claims: normalizedClaims,
          members: normalizedMembers,
          providers: normalizedProviders,
          policies: normalizedPolicies,
          documents: normalizedDocuments,
          payments: normalizedPayments,
          notifications: normalizedNotifications,
          audit: normalizedAudit,
        },
      }
    } catch {
      return {
        usingDemoData: true,
        data: {
          dashboard: demoDashboard,
          claims: demoClaims,
          members: demoMembers,
          providers: demoProviders,
          policies: demoPolicies,
          documents: demoDocuments,
          payments: demoPayments,
          notifications: demoNotifications,
          audit: demoAudit,
        },
      }
    }
  },
}
