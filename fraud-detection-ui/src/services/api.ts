import { apiClient } from '../lib/api';
import type {
  Transaction,
  FraudAlert,
  FraudRule,
  DashboardStatistics,
  CreateTransactionRequest,
  UpdateAlertStatusRequest,
} from '../types';

export const dashboardApi = {
  getStatistics: () =>
    apiClient.get<DashboardStatistics>('/dashboard/statistics'),
  
  getRecentTransactions: (count = 20) =>
    apiClient.get<Transaction[]>(`/dashboard/recent-transactions?count=${count}`),
  
  getRecentAlerts: (count = 10) =>
    apiClient.get<FraudAlert[]>(`/dashboard/recent-alerts?count=${count}`),
};

// Transactions API
export const transactionsApi = {
  getAll: (params?: {
    status?: string;
    riskLevel?: string;
    accountId?: string;
    pageNumber?: number;
    pageSize?: number;
  }) => apiClient.get<Transaction[]>('/transactions', { params }),
  
  getById: (id: string) =>
    apiClient.get<Transaction>(`/transactions/${id}`),
  
  getRecent: (count = 20) =>
    apiClient.get<Transaction[]>(`/transactions/recent?count=${count}`),
  
  create: (data: CreateTransactionRequest) =>
    apiClient.post<Transaction>('/transactions', data),
};

// Fraud Alerts API
export const alertsApi = {
  getAll: (params?: {
    status?: string;
    riskLevel?: string;
    pageNumber?: number;
    pageSize?: number;
  }) => apiClient.get<FraudAlert[]>('/fraudalerts', { params }),
  
  getById: (id: string) =>
    apiClient.get<FraudAlert>(`/fraudalerts/${id}`),
  
  getRecent: (count = 10) =>
    apiClient.get<FraudAlert[]>(`/fraudalerts/recent?count=${count}`),
  
  investigate: (id: string, data: { investigatedBy: string }) =>
    apiClient.put(`/fraudalerts/${id}/investigate`, data),
  
  resolve: (id: string, data: UpdateAlertStatusRequest) =>
    apiClient.put(`/fraudalerts/${id}/resolve`, { resolvedBy: data.reviewedBy, notes: data.notes }),
  
  markFalsePositive: (id: string, data: UpdateAlertStatusRequest) =>
    apiClient.put(`/fraudalerts/${id}/false-positive`, data),
  
  confirmFraud: (id: string, data: UpdateAlertStatusRequest) =>
    apiClient.put(`/fraudalerts/${id}/confirm-fraud`, data),
};

// Fraud Rules API
export const rulesApi = {
  getAll: (params?: { isActive?: boolean; ruleType?: string }) =>
    apiClient.get<FraudRule[]>('/fraudrules', { params }),
  
  getById: (id: string) =>
    apiClient.get<FraudRule>(`/fraudrules/${id}`),
  
  activate: (id: string) =>
    apiClient.put(`/fraudrules/${id}/activate`),
  
  deactivate: (id: string) =>
    apiClient.put(`/fraudrules/${id}/deactivate`),
};