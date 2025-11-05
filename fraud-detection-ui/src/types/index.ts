export interface Transaction {
  id: string;
  accountId: string;
  amount: number;
  currency: string;
  type: string;
  status: string;
  merchantId: string;
  merchantName: string;
  merchantCategory: string;
  latitude: number;
  longitude: number;
  country: string;
  city: string;
  riskLevel: 'None' | 'Low' | 'Medium' | 'High' | 'Critical';
  fraudScore: number;
  transactionDate: string;
  deviceId?: string;
  createdAt: string;
}

export interface FraudAlert {
  id: string;
  transactionId: string;
  ruleName: string;
  status: 'New' | 'Investigating' | 'Resolved' | 'FalsePositive' | 'ConfirmedFraud';
  riskLevel: 'None' | 'Low' | 'Medium' | 'High' | 'Critical';
  score: number;
  message: string;
  details?: string;
  reviewedBy?: string;
  reviewedAt?: string;
  createdAt: string;
}

export interface FraudRule {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
  riskLevel: string;
  priority: number;
  ruleType: string;
  timesTriggered: number;
  lastTriggeredAt?: string;
}

export interface DashboardStatistics {
  totalTransactionsToday: number;
  totalTransactionsThisWeek: number;
  totalTransactionsThisMonth: number;
  activeAlertsCount: number;
  resolvedAlertsToday: number;
  fraudDetectionRate: number;
  totalAmountProcessedToday: number;
  totalAmountFlaggedToday: number;
  highRiskTransactionsToday: number;
  mediumRiskTransactionsToday: number;
  lowRiskTransactionsToday: number;
}

export interface CreateTransactionRequest {
  accountId: string;
  amount: number;
  currency: string;
  type: string;
  merchantId: string;
  merchantName: string;
  merchantCategory: string;
  latitude: number;
  longitude: number;
  country: string;
  city: string;
  ipAddress?: string;
  deviceId?: string;
  description?: string;
}

export interface UpdateAlertStatusRequest {
  reviewedBy: string;
  notes?: string;
}