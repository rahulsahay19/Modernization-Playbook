import { useEffect, useMemo, useState } from 'react'
import {
  Activity,
  Bell,
  Building2,
  CheckCircle2,
  ChevronRight,
  ClipboardCheck,
  CreditCard,
  FileClock,
  FileText,
  LayoutDashboard,
  Menu,
  RefreshCw,
  Search,
  ShieldCheck,
  Users,
  X,
} from 'lucide-react'
import './App.css'
import { claimsApi } from './api/claimsApi'
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
} from './api/types'

type View =
  | 'dashboard'
  | 'claims'
  | 'members'
  | 'providers'
  | 'policies'
  | 'documents'
  | 'payments'
  | 'notifications'
  | 'audit'

type PortalData = {
  dashboard: Dashboard
  claims: Claim[]
  members: Member[]
  providers: Provider[]
  policies: Policy[]
  documents: ClaimDocument[]
  payments: Payment[]
  notifications: NotificationMessage[]
  audit: AuditEntry[]
}

const navigation = [
  { id: 'dashboard' as const, label: 'Overview', icon: LayoutDashboard },
  { id: 'claims' as const, label: 'Claims', icon: ClipboardCheck },
  { id: 'members' as const, label: 'Members', icon: Users },
  { id: 'providers' as const, label: 'Providers', icon: Building2 },
  { id: 'policies' as const, label: 'Policies', icon: ShieldCheck },
  { id: 'documents' as const, label: 'Documents', icon: FileText },
  { id: 'payments' as const, label: 'Payments', icon: CreditCard },
  { id: 'notifications' as const, label: 'Notifications', icon: Bell },
  { id: 'audit' as const, label: 'Audit trail', icon: FileClock },
]

const formatCurrency = (amount: number) =>
  new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 0,
  }).format(amount)

const formatDate = (value?: string | null) =>
  value
    ? new Intl.DateTimeFormat('en-IN', { dateStyle: 'medium' }).format(
        new Date(value),
      )
    : 'Not available'

const normalizeStatus = (status: unknown) =>
  typeof status === 'string' ? status : String(status ?? 'Unknown')

const toneForStatus = (status: unknown) => {
  const normalized = normalizeStatus(status).toLowerCase()
  if (
    normalized.includes('approved') ||
    normalized.includes('paid') ||
    normalized.includes('settled') ||
    normalized.includes('verified') ||
    normalized.includes('sent') ||
    normalized.includes('active')
  )
    return 'success'
  if (normalized.includes('rejected') || normalized.includes('failed'))
    return 'danger'
  if (
    normalized.includes('pending') ||
    normalized.includes('review') ||
    normalized.includes('scheduled') ||
    normalized.includes('uploaded')
  )
    return 'warning'
  return 'neutral'
}

function Status({ children }: { children: unknown }) {
  const label = normalizeStatus(children)
  return <span className={`status status-${toneForStatus(label)}`}>{label}</span>
}

function App() {
  const backendName = import.meta.env.VITE_BACKEND_NAME ?? 'Backend'
  const backendRunCommand =
    import.meta.env.VITE_BACKEND_RUN_COMMAND ??
    'dotnet run --project monolith/src/HealthCare.Claims.Monolith'
  const [activeView, setActiveView] = useState<View>('dashboard')
  const [data, setData] = useState<PortalData | null>(null)
  const [loading, setLoading] = useState(true)
  const [usingDemoData, setUsingDemoData] = useState(false)
  const [search, setSearch] = useState('')
  const [selectedClaimNumber, setSelectedClaimNumber] = useState<string>()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  const loadData = async () => {
    setLoading(true)
    const result = await claimsApi.loadPortal()
    setData(result.data)
    setUsingDemoData(result.usingDemoData)
    setSelectedClaimNumber((current) => current ?? result.data.claims[0]?.claimNumber)
    setLoading(false)
  }

  useEffect(() => {
    void loadData()
  }, [])

  const selectedClaim = data?.claims.find(
    (claim) => claim.claimNumber === selectedClaimNumber,
  )

  const title = navigation.find((item) => item.id === activeView)?.label ?? 'Overview'

  const changeView = (view: View) => {
    setActiveView(view)
    setSearch('')
    setSidebarOpen(false)
  }

  return (
    <div className="app-shell">
      <aside className={`sidebar ${sidebarOpen ? 'sidebar-open' : ''}`}>
        <div className="brand">
          <div className="brand-mark">
            <Activity size={22} />
          </div>
          <div>
            <strong>ClaimSphere</strong>
            <span>Health operations</span>
          </div>
          <button
            className="icon-button sidebar-close"
            onClick={() => setSidebarOpen(false)}
            aria-label="Close navigation"
          >
            <X size={20} />
          </button>
        </div>

        <nav aria-label="Main navigation">
          <p className="nav-label">Workspace</p>
          {navigation.map(({ id, label, icon: Icon }) => (
            <button
              key={id}
              className={`nav-item ${activeView === id ? 'active' : ''}`}
              onClick={() => changeView(id)}
            >
              <Icon size={18} />
              <span>{label}</span>
              {id === 'claims' && data && <b>{data.dashboard.totalClaims}</b>}
            </button>
          ))}
        </nav>

        <div className="system-panel">
          <div className="system-heading">
            <span className={usingDemoData ? 'dot dot-amber' : 'dot'} />
            {usingDemoData ? 'Demo mode' : `${backendName} connected`}
          </div>
          <p>
            {usingDemoData
              ? `Start the ${backendName} API to use live operational data.`
              : backendName === 'Strangler gateway'
                ? 'Gateway routes each capability to its current backend owner.'
                : backendName === 'Modular monolith'
                ? 'Capabilities share one deployment, with module boundaries protected.'
                : 'All capabilities currently share one deployment and database.'}
          </p>
        </div>
      </aside>

      {sidebarOpen && (
        <button
          className="sidebar-backdrop"
          onClick={() => setSidebarOpen(false)}
          aria-label="Close navigation"
        />
      )}

      <main>
        <header className="topbar">
          <div className="heading-group">
            <button
              className="icon-button menu-button"
              onClick={() => setSidebarOpen(true)}
              aria-label="Open navigation"
            >
              <Menu size={20} />
            </button>
            <div>
              <p>Claims operations</p>
              <h1>{title}</h1>
            </div>
          </div>
          <div className="topbar-actions">
            <label className="search-box">
              <Search size={17} />
              <input
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder={`Search ${title.toLowerCase()}`}
                aria-label={`Search ${title.toLowerCase()}`}
              />
            </label>
            <button
              className="icon-button"
              onClick={() => void loadData()}
              title="Refresh data"
              aria-label="Refresh data"
            >
              <RefreshCw size={18} className={loading ? 'spin' : ''} />
            </button>
            <div className="avatar" title="Operations manager">
              RS
            </div>
          </div>
        </header>

        <div className="content">
          {loading && !data ? (
            <LoadingState />
          ) : data ? (
            <>
              {usingDemoData && (
                <div className="notice">
                  <span className="dot dot-amber" />
                  Showing representative data because the API is unavailable.
                  <code>{backendRunCommand}</code>
                </div>
              )}
              {activeView === 'dashboard' && (
                <DashboardView data={data} onOpenClaims={() => changeView('claims')} />
              )}
              {activeView === 'claims' && (
                <ClaimsView
                  claims={data.claims}
                  members={data.members}
                  providers={data.providers}
                  documents={data.documents}
                  payments={data.payments}
                  selectedClaim={selectedClaim}
                  search={search}
                  onSelect={setSelectedClaimNumber}
                />
              )}
              {activeView === 'members' && (
                <MembersView items={data.members} search={search} />
              )}
              {activeView === 'providers' && (
                <ProvidersView items={data.providers} search={search} />
              )}
              {activeView === 'policies' && (
                <PoliciesView items={data.policies} search={search} />
              )}
              {activeView === 'documents' && (
                <DocumentsView items={data.documents} search={search} />
              )}
              {activeView === 'payments' && (
                <PaymentsView items={data.payments} search={search} />
              )}
              {activeView === 'notifications' && (
                <NotificationsView items={data.notifications} search={search} />
              )}
              {activeView === 'audit' && (
                <AuditView items={data.audit} search={search} />
              )}
            </>
          ) : null}
        </div>
      </main>
    </div>
  )
}

function LoadingState() {
  return (
    <div className="loading-state">
      <RefreshCw className="spin" size={24} />
      <p>Loading claims operations...</p>
    </div>
  )
}

function DashboardView({
  data,
  onOpenClaims,
}: {
  data: PortalData
  onOpenClaims: () => void
}) {
  const { dashboard } = data
  const approvalRate = dashboard.requestedAmount
    ? Math.round((dashboard.approvedAmount / dashboard.requestedAmount) * 100)
    : 0

  return (
    <div className="view-stack">
      <section className="metrics-grid" aria-label="Operational metrics">
        <Metric
          label="Claims in system"
          value={dashboard.totalClaims.toString()}
          detail={`${dashboard.pendingDocuments} awaiting documents`}
          icon={ClipboardCheck}
          accent="blue"
        />
        <Metric
          label="Requested amount"
          value={formatCurrency(dashboard.requestedAmount)}
          detail={`${approvalRate}% approved value`}
          icon={Activity}
          accent="teal"
        />
        <Metric
          label="Active members"
          value={dashboard.totalMembers.toString()}
          detail={`${dashboard.totalPolicies} insurance policies`}
          icon={Users}
          accent="violet"
        />
        <Metric
          label="Settled amount"
          value={formatCurrency(dashboard.settledAmount)}
          detail={`${dashboard.scheduledPayments} payments scheduled`}
          icon={CreditCard}
          accent="green"
        />
      </section>

      <section className="dashboard-grid">
        <div className="panel claims-panel">
          <div className="panel-heading">
            <div>
              <p className="eyebrow">Work queue</p>
              <h2>Recent claims</h2>
            </div>
            <button className="text-button" onClick={onOpenClaims}>
              View all <ChevronRight size={16} />
            </button>
          </div>
          <ClaimsTable
            claims={data.claims.slice(0, 5)}
            members={data.members}
            providers={data.providers}
            compact
          />
        </div>

        <div className="panel">
          <div className="panel-heading">
            <div>
              <p className="eyebrow">Distribution</p>
              <h2>Claims by status</h2>
            </div>
          </div>
          <div className="status-chart">
            {dashboard.claimsByStatus.map((item, index) => {
              const width = Math.max(
                8,
                (item.count / Math.max(dashboard.totalClaims, 1)) * 100,
              )
              return (
                <div className="chart-row" key={item.status}>
                  <div>
                    <span>{item.status}</span>
                    <b>{item.count}</b>
                  </div>
                  <div className="chart-track">
                    <span
                      style={{ width: `${width}%` }}
                      className={`chart-fill chart-${index % 4}`}
                    />
                  </div>
                </div>
              )
            })}
          </div>
        </div>
      </section>

      <section className="dashboard-grid lower-grid">
        <div className="panel">
          <div className="panel-heading">
            <div>
              <p className="eyebrow">Dependencies</p>
              <h2>Shared platform load</h2>
            </div>
          </div>
          <div className="load-grid">
            <LoadItem
              label="Documents"
              value={dashboard.uploadedDocuments}
              detail={`${dashboard.pendingDocuments} claims pending`}
            />
            <LoadItem
              label="Providers"
              value={dashboard.totalProviders}
              detail="Across the shared database"
            />
            <LoadItem
              label="Notifications"
              value={dashboard.notificationsSent}
              detail="Sent by the monolith"
            />
          </div>
        </div>
        <div className="panel">
          <div className="panel-heading">
            <div>
              <p className="eyebrow">Latest activity</p>
              <h2>Audit trail</h2>
            </div>
          </div>
          <div className="activity-list">
            {data.audit.slice(0, 4).map((entry) => (
              <div className="activity-item" key={entry.auditId}>
                <span className="activity-icon">
                  <CheckCircle2 size={16} />
                </span>
                <div>
                  <strong>{entry.action}</strong>
                  <p>{entry.description}</p>
                </div>
                <time>{formatDate(entry.performedAt)}</time>
              </div>
            ))}
          </div>
        </div>
      </section>
    </div>
  )
}

function Metric({
  label,
  value,
  detail,
  icon: Icon,
  accent,
}: {
  label: string
  value: string
  detail: string
  icon: typeof Activity
  accent: string
}) {
  return (
    <article className="metric">
      <div className={`metric-icon metric-${accent}`}>
        <Icon size={20} />
      </div>
      <p>{label}</p>
      <strong>{value}</strong>
      <span>{detail}</span>
    </article>
  )
}

function LoadItem({
  label,
  value,
  detail,
}: {
  label: string
  value: number
  detail: string
}) {
  return (
    <div className="load-item">
      <strong>{value}</strong>
      <div>
        <b>{label}</b>
        <span>{detail}</span>
      </div>
    </div>
  )
}

function ClaimsView({
  claims,
  members,
  providers,
  documents,
  payments,
  selectedClaim,
  search,
  onSelect,
}: {
  claims: Claim[]
  members: Member[]
  providers: Provider[]
  documents: ClaimDocument[]
  payments: Payment[]
  selectedClaim?: Claim
  search: string
  onSelect: (claimNumber: string) => void
}) {
  const filtered = useMemo(
    () =>
      claims.filter((claim) =>
        JSON.stringify(claim).toLowerCase().includes(search.toLowerCase()),
      ),
    [claims, search],
  )

  return (
    <div className="claims-layout">
      <div className="panel table-panel">
        <div className="panel-heading">
          <div>
            <p className="eyebrow">Processing queue</p>
            <h2>{filtered.length} claims</h2>
          </div>
          <select aria-label="Filter claims by status" defaultValue="all">
            <option value="all">All statuses</option>
            <option>PendingDocuments</option>
            <option>UnderReview</option>
            <option>Approved</option>
            <option>Paid</option>
          </select>
        </div>
        <ClaimsTable
          claims={filtered}
          members={members}
          providers={providers}
          selected={selectedClaim?.claimNumber}
          onSelect={onSelect}
        />
      </div>
      <ClaimDetail
        claim={selectedClaim}
        member={members.find((item) => item.memberId === selectedClaim?.memberId)}
        provider={providers.find(
          (item) => item.providerId === selectedClaim?.providerId,
        )}
        documents={documents.filter(
          (item) => item.claimNumber === selectedClaim?.claimNumber,
        )}
        payment={payments.find(
          (item) => item.claimNumber === selectedClaim?.claimNumber,
        )}
      />
    </div>
  )
}

function ClaimsTable({
  claims,
  members,
  providers,
  compact = false,
  selected,
  onSelect,
}: {
  claims: Claim[]
  members: Member[]
  providers: Provider[]
  compact?: boolean
  selected?: string
  onSelect?: (claimNumber: string) => void
}) {
  return (
    <div className="table-scroll">
      <table>
        <thead>
          <tr>
            <th>Claim</th>
            <th>Member</th>
            {!compact && <th>Provider</th>}
            <th>Amount</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {claims.map((claim) => (
            <tr
              key={claim.claimNumber}
              className={`${onSelect ? 'clickable-row' : ''} ${
                selected === claim.claimNumber ? 'selected-row' : ''
              }`}
              onClick={() => onSelect?.(claim.claimNumber)}
            >
              <td>
                <strong>{claim.claimNumber}</strong>
                <span>{formatDate(claim.serviceDate)}</span>
              </td>
              <td>
                {members.find((item) => item.memberId === claim.memberId)?.fullName ??
                  claim.memberId}
              </td>
              {!compact && (
                <td>
                  {providers.find((item) => item.providerId === claim.providerId)
                    ?.name ?? claim.providerId}
                </td>
              )}
              <td className="money">{formatCurrency(claim.requestedAmount)}</td>
              <td>
                <Status>{claim.status}</Status>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {!claims.length && <EmptyState />}
    </div>
  )
}

function ClaimDetail({
  claim,
  member,
  provider,
  documents,
  payment,
}: {
  claim?: Claim
  member?: Member
  provider?: Provider
  documents: ClaimDocument[]
  payment?: Payment
}) {
  if (!claim) {
    return (
      <aside className="panel claim-detail">
        <EmptyState message="Select a claim to inspect its workflow." />
      </aside>
    )
  }

  return (
    <aside className="panel claim-detail">
      <div className="detail-header">
        <div>
          <p className="eyebrow">Claim detail</p>
          <h2>{claim.claimNumber}</h2>
        </div>
        <Status>{claim.status}</Status>
      </div>
      <div className="amount-block">
        <span>Requested amount</span>
        <strong>{formatCurrency(claim.requestedAmount)}</strong>
        <small>
          Approved: {formatCurrency(claim.approvedAmount)} · {claim.lines.length}{' '}
          line items
        </small>
      </div>
      <dl className="detail-list">
        <div>
          <dt>Member</dt>
          <dd>{member?.fullName ?? claim.memberId}</dd>
        </div>
        <div>
          <dt>Provider</dt>
          <dd>{provider?.name ?? claim.providerId}</dd>
        </div>
        <div>
          <dt>Service date</dt>
          <dd>{formatDate(claim.serviceDate)}</dd>
        </div>
      </dl>
      <div className="detail-section">
        <h3>Claim lines</h3>
        {claim.lines.map((line) => (
          <div className="line-item" key={line.code}>
            <div>
              <b>{line.description}</b>
              <span>{line.code}</span>
            </div>
            <strong>{formatCurrency(line.amount)}</strong>
          </div>
        ))}
      </div>
      <div className="detail-section">
        <h3>Workflow</h3>
        <div className="workflow-item">
          <FileText size={17} />
          <div>
            <b>{documents.length} documents</b>
            <span>
              {documents.every((item) => item.status === 'Verified')
                ? 'Verification complete'
                : 'Verification required'}
            </span>
          </div>
        </div>
        <div className="workflow-item">
          <CreditCard size={17} />
          <div>
            <b>{payment ? `Payment ${payment.status}` : 'No payment scheduled'}</b>
            <span>{payment?.settlementReference ?? 'Awaiting claim approval'}</span>
          </div>
        </div>
      </div>
    </aside>
  )
}

function MembersView({ items, search }: { items: Member[]; search: string }) {
  const filtered = filterItems(items, search)
  return (
    <EntityTable
      title={`${filtered.length} members`}
      eyebrow="Covered lives"
      headers={['Member', 'Policy', 'Date of birth', 'Contact', 'Status']}
      rows={filtered.map((item) => [
        <PrimaryCell key="member" primary={item.fullName} secondary={item.memberId} />,
        item.policyNumber,
        formatDate(item.dateOfBirth),
        <PrimaryCell key="contact" primary={item.email} secondary={item.mobileNumber} />,
        <Status key="status">{item.isActive ? 'Active' : 'Inactive'}</Status>,
      ])}
    />
  )
}

function ProvidersView({ items, search }: { items: Provider[]; search: string }) {
  const filtered = filterItems(items, search)
  return (
    <EntityTable
      title={`${filtered.length} providers`}
      eyebrow="Care network"
      headers={['Provider', 'Network tier', 'City', 'Status']}
      rows={filtered.map((item) => [
        <PrimaryCell key="provider" primary={item.name} secondary={item.providerId} />,
        item.networkTier,
        item.city,
        <Status key="status">{item.isActive ? 'Active' : 'Inactive'}</Status>,
      ])}
    />
  )
}

function PoliciesView({ items, search }: { items: Policy[]; search: string }) {
  const filtered = filterItems(items, search)
  return (
    <EntityTable
      title={`${filtered.length} policies`}
      eyebrow="Insurance products"
      headers={['Policy', 'Insurer', 'Annual limit', 'Deductible', 'Status']}
      rows={filtered.map((item) => [
        <PrimaryCell
          key="policy"
          primary={item.policyNumber}
          secondary={item.planName}
        />,
        item.insuranceProviderName,
        formatCurrency(item.annualLimit),
        formatCurrency(item.deductible),
        <Status key="status">{item.isActive ? 'Active' : 'Inactive'}</Status>,
      ])}
    />
  )
}

function DocumentsView({
  items,
  search,
}: {
  items: ClaimDocument[]
  search: string
}) {
  const filtered = filterItems(items, search)
  return (
    <EntityTable
      title={`${filtered.length} documents`}
      eyebrow="Evidence review"
      headers={['Document', 'Claim', 'Type', 'Uploaded', 'Status']}
      rows={filtered.map((item) => [
        <PrimaryCell
          key="document"
          primary={item.fileName}
          secondary={item.documentId}
        />,
        item.claimNumber,
        item.documentType,
        formatDate(item.uploadedAt),
        <Status key="status">{item.status}</Status>,
      ])}
    />
  )
}

function PaymentsView({ items, search }: { items: Payment[]; search: string }) {
  const filtered = filterItems(items, search)
  return (
    <EntityTable
      title={`${filtered.length} payments`}
      eyebrow="Settlement desk"
      headers={['Payment', 'Claim', 'Amount', 'Mode', 'Schedule', 'Status']}
      rows={filtered.map((item) => [
        <PrimaryCell
          key="payment"
          primary={item.paymentId}
          secondary={item.settlementReference ?? 'Reference pending'}
        />,
        item.claimNumber,
        formatCurrency(item.amount),
        item.paymentMode,
        formatDate(item.scheduledDate),
        <Status key="status">{item.status}</Status>,
      ])}
    />
  )
}

function NotificationsView({
  items,
  search,
}: {
  items: NotificationMessage[]
  search: string
}) {
  const filtered = filterItems(items, search)
  return (
    <EntityTable
      title={`${filtered.length} notifications`}
      eyebrow="Member communication"
      headers={['Message', 'Recipient', 'Channel', 'Created', 'Status']}
      rows={filtered.map((item) => [
        <PrimaryCell
          key="notification"
          primary={item.subject}
          secondary={item.notificationId}
        />,
        item.recipient,
        item.channel,
        formatDate(item.createdAt),
        <Status key="status">{item.status}</Status>,
      ])}
    />
  )
}

function AuditView({ items, search }: { items: AuditEntry[]; search: string }) {
  const filtered = filterItems(items, search)
  return (
    <EntityTable
      title={`${filtered.length} audit entries`}
      eyebrow="System accountability"
      headers={['Action', 'Entity', 'Description', 'Performed by', 'Timestamp']}
      rows={filtered.map((item) => [
        <PrimaryCell key="action" primary={item.action} secondary={item.auditId} />,
        `${item.entityType} · ${item.entityId}`,
        item.description,
        item.performedBy,
        formatDate(item.performedAt),
      ])}
    />
  )
}

function EntityTable({
  title,
  eyebrow,
  headers,
  rows,
}: {
  title: string
  eyebrow: string
  headers: string[]
  rows: React.ReactNode[][]
}) {
  return (
    <section className="panel table-panel">
      <div className="panel-heading">
        <div>
          <p className="eyebrow">{eyebrow}</p>
          <h2>{title}</h2>
        </div>
      </div>
      <div className="table-scroll">
        <table>
          <thead>
            <tr>
              {headers.map((header) => (
                <th key={header}>{header}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {rows.map((row, rowIndex) => (
              <tr key={rowIndex}>
                {row.map((cell, cellIndex) => (
                  <td key={cellIndex}>{cell}</td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
        {!rows.length && <EmptyState />}
      </div>
    </section>
  )
}

function PrimaryCell({
  primary,
  secondary,
}: {
  primary: string
  secondary: string
}) {
  return (
    <span className="primary-cell">
      <strong>{primary}</strong>
      <span>{secondary}</span>
    </span>
  )
}

function EmptyState({ message = 'No records match this search.' }: { message?: string }) {
  return (
    <div className="empty-state">
      <Search size={22} />
      <p>{message}</p>
    </div>
  )
}

function filterItems<T>(items: T[], search: string) {
  const query = search.trim().toLowerCase()
  if (!query) return items
  return items.filter((item) => JSON.stringify(item).toLowerCase().includes(query))
}

export default App
