import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { dashboardApi, transactionsApi, alertsApi } from '../services/api';
import { formatCurrency, formatDate, getRiskColor, getStatusColor } from '../lib/utils';
import { Activity, AlertTriangle, DollarSign, TrendingUp } from 'lucide-react';
import { Link } from 'react-router-dom';

export function Dashboard() {
  const { data: stats } = useQuery({
    queryKey: ['dashboard', 'statistics'],
    queryFn: () => dashboardApi.getStatistics().then(res => res.data),
    refetchInterval: 5000,
  });

  const { data: recentTransactions } = useQuery({
    queryKey: ['dashboard', 'recentTransactions'],
    queryFn: () => transactionsApi.getRecent(20).then(res => res.data),
    refetchInterval: 5000, 
  });

  const { data: recentAlerts } = useQuery({
    queryKey: ['dashboard', 'recentAlerts'],
    queryFn: () => alertsApi.getRecent(5).then(res => res.data),
    refetchInterval: 5000,
  });

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-3xl font-bold tracking-tight text-gray-900">Dashboard</h2>
        <p className="text-gray-500 mt-1">Real-time fraud detection monitoring</p>
      </div>

      {/* Metrics Cards */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2 space-y-0">
            <CardTitle className="text-sm font-medium">Transactions Today</CardTitle>
            <Activity className="h-4 w-4 text-gray-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats?.totalTransactionsToday ?? 0}</div>
            <p className="text-xs text-gray-500 mt-1">
              {stats?.totalTransactionsThisWeek ?? 0} this week
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2 space-y-0">
            <CardTitle className="text-sm font-medium">Active Alerts</CardTitle>
            <AlertTriangle className="h-4 w-4 text-red-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">
              {stats?.activeAlertsCount ?? 0}
            </div>
            <p className="text-xs text-gray-500 mt-1">
              {stats?.resolvedAlertsToday ?? 0} resolved today
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2 space-y-0">
            <CardTitle className="text-sm font-medium">Avg Fraud Score</CardTitle>
            <TrendingUp className="h-4 w-4 text-yellow-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {stats ? (
                <>
                  {(((stats.highRiskTransactionsToday * 80 + stats.mediumRiskTransactionsToday * 40) / 
                     (stats.totalTransactionsToday || 1))).toFixed(1)}
                </>
              ) : '0.0'}
            </div>
            <p className="text-xs text-gray-500 mt-1">
              Detection rate: {stats?.fraudDetectionRate?.toFixed(1) ?? 0}%
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2 space-y-0">
            <CardTitle className="text-sm font-medium">Amount Processed</CardTitle>
            <DollarSign className="h-4 w-4 text-green-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {formatCurrency(stats?.totalAmountProcessedToday ?? 0)}
            </div>
            <p className="text-xs text-red-500 mt-1">
              {formatCurrency(stats?.totalAmountFlaggedToday ?? 0)} flagged
            </p>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        {/* Recent Transactions */}
        <Card>
          <CardHeader>
            <CardTitle>Recent Transactions</CardTitle>
            <CardDescription>Latest 20 transactions in the system</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {recentTransactions?.slice(0, 20).map((tx) => (
                <Link
                  key={tx.id}
                  to={`/transactions?highlight=${tx.id}`}
                  className="flex items-center justify-between p-3 rounded-lg border hover:bg-gray-50 transition-colors"
                >
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2">
                      <p className="text-sm font-medium text-gray-900 truncate">
                        {tx.merchantName}
                      </p>
                      <Badge className={getRiskColor(tx.riskLevel) + ' text-xs'}>
                        {tx.riskLevel}
                      </Badge>
                    </div>
                    <p className="text-xs text-gray-500 mt-1">
                      {tx.accountId} • {formatDate(tx.createdAt)}
                    </p>
                  </div>
                  <div className="text-right ml-4">
                    <p className="text-sm font-semibold text-gray-900">
                      {formatCurrency(tx.amount, tx.currency)}
                    </p>
                    <p className="text-xs text-gray-500">
                      Score: {tx.fraudScore.toFixed(1)}
                    </p>
                  </div>
                </Link>
              ))}
              {!recentTransactions?.length && (
                <p className="text-sm text-gray-500 text-center py-8">
                  No transactions yet
                </p>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Recent Alerts */}
        <Card>
          <CardHeader>
            <CardTitle>Active Fraud Alerts</CardTitle>
            <CardDescription>Recent fraud detection alerts</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {recentAlerts?.map((alert) => (
                <Link
                  key={alert.id}
                  to={`/alerts?highlight=${alert.id}`}
                  className="flex items-center justify-between p-3 rounded-lg border hover:bg-gray-50 transition-colors"
                >
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2">
                      <p className="text-sm font-medium text-gray-900 truncate">
                        {alert.ruleName}
                      </p>
                      <Badge className={getStatusColor(alert.status) + ' text-xs'}>
                        {alert.status}
                      </Badge>
                    </div>
                    <p className="text-xs text-gray-500 mt-1 truncate">
                      {alert.message}
                    </p>
                    <p className="text-xs text-gray-400 mt-1">
                      {formatDate(alert.createdAt)}
                    </p>
                  </div>
                  <div className="ml-4">
                    <Badge className={getRiskColor(alert.riskLevel)}>
                      {alert.score.toFixed(0)}
                    </Badge>
                  </div>
                </Link>
              ))}
              {!recentAlerts?.length && (
                <p className="text-sm text-gray-500 text-center py-8">
                  No alerts yet
                </p>
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}