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

export const demoPolicies: Policy[] = [
  {
    policyNumber: 'POL-1001',
    insuranceProviderName: 'Star Health and Allied Insurance Co. Ltd.',
    planName: 'Family Health Optima Insurance Plan',
    annualLimit: 250000,
    deductible: 5000,
    isActive: true,
  },
  {
    policyNumber: 'POL-2002',
    insuranceProviderName: 'HDFC ERGO General Insurance Co. Ltd.',
    planName: 'Optima Secure Health Insurance',
    annualLimit: 500000,
    deductible: 2500,
    isActive: true,
  },
]

export const demoMembers: Member[] = [
  {
    memberId: 'MEM-1001',
    fullName: 'Aarav Mehta',
    policyNumber: 'POL-1001',
    dateOfBirth: '1988-04-12',
    email: 'aarav.mehta@example.com',
    mobileNumber: '+91 98765 43210',
    isActive: true,
  },
  {
    memberId: 'MEM-2002',
    fullName: 'Maya Iyer',
    policyNumber: 'POL-2002',
    dateOfBirth: '1992-09-02',
    email: 'maya.iyer@example.com',
    mobileNumber: '+91 98123 45678',
    isActive: true,
  },
  {
    memberId: 'MEM-3014',
    fullName: 'Kabir Singh',
    policyNumber: 'POL-2002',
    dateOfBirth: '1979-11-23',
    email: 'kabir.singh@example.com',
    mobileNumber: '+91 99881 14420',
    isActive: true,
  },
]

export const demoProviders: Provider[] = [
  {
    providerId: 'PRV-101',
    name: 'Apollo Hospitals',
    networkTier: 'Gold',
    city: 'Chennai',
    isActive: true,
  },
  {
    providerId: 'PRV-202',
    name: 'Dr. Lal PathLabs',
    networkTier: 'Silver',
    city: 'Gurugram',
    isActive: true,
  },
  {
    providerId: 'PRV-318',
    name: 'Fortis Healthcare',
    networkTier: 'Gold',
    city: 'Bengaluru',
    isActive: true,
  },
]

export const demoClaims: Claim[] = [
  {
    claimNumber: 'CLM-90001',
    memberId: 'MEM-1001',
    providerId: 'PRV-101',
    serviceDate: '2026-06-10',
    requestedAmount: 18500,
    approvedAmount: 0,
    status: 'PendingDocuments',
    lines: [
      { code: 'ER-100', description: 'Emergency consultation', amount: 8500 },
      { code: 'LAB-210', description: 'Blood panel', amount: 10000 },
    ],
    documentIds: ['DOC-70001'],
    createdAt: '2026-06-12T09:30:00Z',
  },
  {
    claimNumber: 'CLM-90002',
    memberId: 'MEM-2002',
    providerId: 'PRV-202',
    serviceDate: '2026-06-15',
    requestedAmount: 7200,
    approvedAmount: 6500,
    status: 'Approved',
    lines: [{ code: 'DIA-110', description: 'Diagnostic screening', amount: 7200 }],
    documentIds: ['DOC-70002'],
    createdAt: '2026-06-16T11:20:00Z',
  },
  {
    claimNumber: 'CLM-90003',
    memberId: 'MEM-3014',
    providerId: 'PRV-318',
    serviceDate: '2026-06-18',
    requestedAmount: 64200,
    approvedAmount: 58500,
    status: 'Paid',
    lines: [
      { code: 'SUR-410', description: 'Day-care procedure', amount: 52000 },
      { code: 'MED-125', description: 'Prescribed medicines', amount: 12200 },
    ],
    documentIds: ['DOC-70003'],
    createdAt: '2026-06-18T14:45:00Z',
  },
  {
    claimNumber: 'CLM-90004',
    memberId: 'MEM-1001',
    providerId: 'PRV-202',
    serviceDate: '2026-06-20',
    requestedAmount: 4800,
    approvedAmount: 0,
    status: 'UnderReview',
    lines: [{ code: 'LAB-270', description: 'Thyroid profile', amount: 4800 }],
    documentIds: ['DOC-70004'],
    createdAt: '2026-06-21T08:10:00Z',
  },
]

export const demoDocuments: ClaimDocument[] = demoClaims.map((claim, index) => ({
  documentId: `DOC-${70001 + index}`,
  claimNumber: claim.claimNumber,
  fileName: index === 0 ? 'hospital-bill.pdf' : `claim-evidence-${index + 1}.pdf`,
  documentType: index === 0 ? 'HospitalBill' : 'MedicalEvidence',
  storagePath: `/SimulatedDocumentStore/${claim.claimNumber}/evidence.pdf`,
  status: index === 0 ? 'Uploaded' : 'Verified',
  uploadedAt: claim.createdAt,
}))

export const demoPayments: Payment[] = [
  {
    paymentId: 'PAY-48001',
    claimNumber: 'CLM-90003',
    amount: 58500,
    status: 'Settled',
    paymentMode: 'NEFT',
    settlementReference: 'UTR-HDFC-8349201',
    scheduledDate: '2026-06-20',
    settledDate: '2026-06-21',
  },
  {
    paymentId: 'PAY-48002',
    claimNumber: 'CLM-90002',
    amount: 6500,
    status: 'Scheduled',
    paymentMode: 'NEFT',
    scheduledDate: '2026-06-25',
  },
]

export const demoNotifications: NotificationMessage[] = [
  {
    notificationId: 'NOT-35001',
    channel: 'Email',
    status: 'Sent',
    recipient: 'maya.iyer@example.com',
    subject: 'Claim CLM-90002 approved',
    body: 'Your claim has been approved.',
    createdAt: '2026-06-19T10:00:00Z',
    sentAt: '2026-06-19T10:00:02Z',
  },
  {
    notificationId: 'NOT-35002',
    channel: 'Sms',
    status: 'Sent',
    recipient: '+91 99881 14420',
    subject: 'Claim payment settled',
    body: 'Payment for CLM-90003 has been settled.',
    createdAt: '2026-06-21T16:00:00Z',
    sentAt: '2026-06-21T16:00:01Z',
  },
]

export const demoAudit: AuditEntry[] = [
  {
    auditId: 'AUD-10004',
    entityType: 'Claim',
    entityId: 'CLM-90004',
    action: 'ReviewStarted',
    description: 'Claim moved to medical review.',
    performedBy: 'legacy-system',
    performedAt: '2026-06-22T09:20:00Z',
  },
  {
    auditId: 'AUD-10003',
    entityType: 'Payment',
    entityId: 'PAY-48001',
    action: 'Settled',
    description: 'NEFT settlement completed for claim CLM-90003.',
    performedBy: 'legacy-system',
    performedAt: '2026-06-21T16:00:00Z',
  },
  {
    auditId: 'AUD-10002',
    entityType: 'Claim',
    entityId: 'CLM-90002',
    action: 'Approved',
    description: 'Claim approved after document verification.',
    performedBy: 'legacy-system',
    performedAt: '2026-06-19T10:00:00Z',
  },
  {
    auditId: 'AUD-10001',
    entityType: 'Claim',
    entityId: 'CLM-90001',
    action: 'Submitted',
    description: 'Claim submitted and documents requested.',
    performedBy: 'legacy-system',
    performedAt: '2026-06-12T09:30:00Z',
  },
]

export const demoDashboard: Dashboard = {
  totalPolicies: demoPolicies.length,
  totalMembers: demoMembers.length,
  totalProviders: demoProviders.length,
  totalClaims: demoClaims.length,
  claimsByStatus: [
    { status: 'Pending documents', count: 1 },
    { status: 'Under review', count: 1 },
    { status: 'Approved', count: 1 },
    { status: 'Paid', count: 1 },
  ],
  documentsByStatus: [
    { status: 'Uploaded', count: 1 },
    { status: 'Verified', count: 3 },
  ],
  paymentsByStatus: [
    { status: 'Scheduled', count: 1 },
    { status: 'Settled', count: 1 },
  ],
  requestedAmount: demoClaims.reduce((sum, claim) => sum + claim.requestedAmount, 0),
  approvedAmount: demoClaims.reduce((sum, claim) => sum + claim.approvedAmount, 0),
  settledAmount: 58500,
  pendingDocuments: 1,
  uploadedDocuments: demoDocuments.length,
  scheduledPayments: 1,
  notificationsSent: demoNotifications.length,
}
