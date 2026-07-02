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
} from './types'

const baseUrl = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '')

async function get<T>(path: string): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    headers: { Accept: 'application/json' },
  })

  if (!response.ok) {
    throw new Error(`API request failed with ${response.status}`)
  }

  return response.json() as Promise<T>
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
        get<Dashboard>('/api/reports/operations'),
        get<Claim[]>('/api/claims'),
        get<Member[]>('/api/members'),
        get<Provider[]>('/api/providers'),
        get<Policy[]>('/api/policies'),
        get<ClaimDocument[]>('/api/documents'),
        get<Payment[]>('/api/payments'),
        get<NotificationMessage[]>('/api/notifications'),
        get<AuditEntry[]>('/api/audit'),
      ])

      return {
        usingDemoData: false,
        data: {
          dashboard,
          claims,
          members,
          providers,
          policies,
          documents,
          payments,
          notifications,
          audit,
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
