export type Policy = {
  policyNumber: string
  insuranceProviderName: string
  planName: string
  annualLimit: number
  deductible: number
  isActive: boolean
}

export type Member = {
  memberId: string
  fullName: string
  policyNumber: string
  dateOfBirth: string
  email: string
  mobileNumber: string
  isActive: boolean
}

export type Provider = {
  providerId: string
  name: string
  networkTier: string
  city: string
  isActive: boolean
}

export type ClaimLine = {
  code: string
  description: string
  amount: number
}

export type Claim = {
  claimNumber: string
  memberId: string
  providerId: string
  serviceDate: string
  requestedAmount: number
  approvedAmount: number
  status: string
  lines: ClaimLine[]
  documentIds: string[]
  createdAt: string
}

export type ClaimDocument = {
  documentId: string
  claimNumber: string
  fileName: string
  documentType: string
  storagePath: string
  status: string
  rejectionReason?: string | null
  uploadedAt: string
}

export type Payment = {
  paymentId: string
  claimNumber: string
  amount: number
  status: string
  paymentMode: string
  settlementReference?: string | null
  failureReason?: string | null
  scheduledDate: string
  settledDate?: string | null
}

export type NotificationMessage = {
  notificationId: string
  channel: string
  status: string
  recipient: string
  subject: string
  body: string
  createdAt: string
  sentAt?: string | null
}

export type AuditEntry = {
  auditId: string
  entityType: string
  entityId: string
  action: string
  description: string
  performedBy: string
  performedAt: string
}

export type StatusCount = {
  status: string
  count: number
}

export type Dashboard = {
  totalPolicies: number
  totalMembers: number
  totalProviders: number
  totalClaims: number
  claimsByStatus: StatusCount[]
  documentsByStatus: StatusCount[]
  paymentsByStatus: StatusCount[]
  requestedAmount: number
  approvedAmount: number
  settledAmount: number
  pendingDocuments: number
  uploadedDocuments: number
  scheduledPayments: number
  notificationsSent: number
}
