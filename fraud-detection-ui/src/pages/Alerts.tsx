import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { alertsApi } from '../services/api';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Button } from '../components/ui/button';
import { formatDateLong, getRiskColor, getStatusColor } from '../lib/utils';
import { useToast } from '../hooks/use-toast';
import { AlertTriangle, CheckCircle, XCircle, Eye } from 'lucide-react';

export function Alerts() {
  const [filters, setFilters] = useState({ status: '', riskLevel: '' });
  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: alerts, isLoading } = useQuery({
    queryKey: ['alerts', filters],
    queryFn: () => alertsApi.getAll({
      status: filters.status || undefined,
      riskLevel: filters.riskLevel || undefined,
      pageSize: 50,
    }).then(res => res.data),
    refetchInterval: 10000,
  });

  const investigateMutation = useMutation({
    mutationFn: (alertId: string) => 
      alertsApi.investigate(alertId, { investigatedBy: 'System Admin' }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
      toast({ title: 'Alert marked as investigating' });
    },
  });

  const resolveMutation = useMutation({
    mutationFn: (alertId: string) => 
      alertsApi.resolve(alertId, { reviewedBy: 'System Admin', notes: 'Resolved via dashboard' }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
      toast({ title: 'Alert resolved successfully' });
    },
  });

  const falsePositiveMutation = useMutation({
    mutationFn: (alertId: string) => 
      alertsApi.markFalsePositive(alertId, { reviewedBy: 'System Admin', notes: 'False positive' }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
      toast({ title: 'Alert marked as false positive' });
    },
  });

  const handleClearFilters = () => {
    setFilters({ status: '', riskLevel: '' });
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight text-gray-900">Fraud Alerts</h2>
        <p className="text-gray-500 mt-1">Review and manage fraud detection alerts</p>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Filters</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap gap-4">
            <select
              value={filters.status}
              onChange={(e) => setFilters({ ...filters, status: e.target.value })}
              className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">All Statuses</option>
              <option value="New">New</option>
              <option value="Investigating">Investigating</option>
              <option value="Resolved">Resolved</option>
              <option value="FalsePositive">False Positive</option>
              <option value="ConfirmedFraud">Confirmed Fraud</option>
            </select>

            <select
              value={filters.riskLevel}
              onChange={(e) => setFilters({ ...filters, riskLevel: e.target.value })}
              className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">All Risk Levels</option>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>

            {(filters.status || filters.riskLevel) && (
              <Button variant="outline" onClick={handleClearFilters} size="sm">
                Clear Filters
              </Button>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Alerts List */}
      <Card>
        <CardHeader>
          <CardTitle>{alerts?.length ?? 0} Alerts</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8 text-gray-500">Loading alerts...</div>
          ) : alerts && alerts.length > 0 ? (
            <div className="space-y-3">
              {alerts.map((alert) => (
                <div
                  key={alert.id}
                  className="border rounded-lg p-4 hover:bg-gray-50 transition-colors"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-2">
                        <AlertTriangle className="h-5 w-5 text-red-500 shrink-0" />
                        <h3 className="font-semibold text-gray-900">{alert.ruleName}</h3>
                        <Badge className={getRiskColor(alert.riskLevel)}>
                          {alert.riskLevel}
                        </Badge>
                        <Badge className={getStatusColor(alert.status)}>
                          {alert.status}
                        </Badge>
                        <span className="text-sm font-semibold text-gray-700">
                          Score: {alert.score.toFixed(0)}
                        </span>
                      </div>
                      
                      <p className="text-sm text-gray-700 mb-2">{alert.message}</p>
                      
                      {alert.details && (
                        <p className="text-xs text-gray-500 mb-2 font-mono">{alert.details}</p>
                      )}
                      
                      <div className="flex items-center gap-4 text-xs text-gray-500">
                        <span>Transaction: {alert.transactionId.substring(0, 8)}...</span>
                        <span>Alert ID: {alert.id.substring(0, 8)}...</span>
                        <span>{formatDateLong(alert.createdAt)}</span>
                      </div>

                      {alert.reviewedBy && (
                        <p className="text-xs text-gray-500 mt-2">
                          Reviewed by: {alert.reviewedBy} on {alert.reviewedAt && formatDateLong(alert.reviewedAt)}
                        </p>
                      )}
                    </div>

                    {/* Action Buttons */}
                    <div className="flex flex-col gap-2">
                      {alert.status === 'New' && (
                        <>
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={() => investigateMutation.mutate(alert.id)}
                            disabled={investigateMutation.isPending}
                          >
                            <Eye className="h-4 w-4 mr-1" />
                            Investigate
                          </Button>
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={() => falsePositiveMutation.mutate(alert.id)}
                            disabled={falsePositiveMutation.isPending}
                          >
                            <XCircle className="h-4 w-4 mr-1" />
                            False Positive
                          </Button>
                        </>
                      )}
                      {alert.status === 'Investigating' && (
                        <>
                          <Button
                            size="sm"
                            variant="default"
                            onClick={() => resolveMutation.mutate(alert.id)}
                            disabled={resolveMutation.isPending}
                          >
                            <CheckCircle className="h-4 w-4 mr-1" />
                            Resolve
                          </Button>
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={() => falsePositiveMutation.mutate(alert.id)}
                            disabled={falsePositiveMutation.isPending}
                          >
                            <XCircle className="h-4 w-4 mr-1" />
                            False Positive
                          </Button>
                        </>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-8 text-gray-500">
              No alerts found. Try adjusting your filters.
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}