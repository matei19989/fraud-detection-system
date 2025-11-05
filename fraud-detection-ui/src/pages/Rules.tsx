import { useQuery } from '@tanstack/react-query';
import { rulesApi } from '../services/api';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { getRiskColor } from '../lib/utils';
import { Shield, CheckCircle, XCircle } from 'lucide-react';

export function Rules() {
  const { data: rules, isLoading } = useQuery({
    queryKey: ['rules'],
    queryFn: () => rulesApi.getAll().then(res => res.data),
  });

  const activeRules = rules?.filter(r => r.isActive) ?? [];
  const inactiveRules = rules?.filter(r => !r.isActive) ?? [];

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight text-gray-900">Fraud Rules</h2>
        <p className="text-gray-500 mt-1">
          View configured fraud detection rules ({activeRules.length} active, {inactiveRules.length} inactive)
        </p>
      </div>

      {/* Active Rules */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <CheckCircle className="h-5 w-5 text-green-500" />
            Active Rules ({activeRules.length})
          </CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8 text-gray-500">Loading rules...</div>
          ) : activeRules.length > 0 ? (
            <div className="space-y-3">
              {activeRules.map((rule) => (
                <div
                  key={rule.id}
                  className="border rounded-lg p-4 hover:bg-gray-50 transition-colors"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <Shield className="h-5 w-5 text-blue-500 shrink-0" />
                        <h3 className="font-semibold text-gray-900">{rule.name}</h3>
                        <Badge className={getRiskColor(rule.riskLevel)}>
                          {rule.riskLevel}
                        </Badge>
                        <Badge variant="outline" className="text-xs">
                          Priority: {rule.priority}
                        </Badge>
                      </div>
                      
                      <p className="text-sm text-gray-700 mb-3">{rule.description}</p>
                      
                      <div className="flex items-center gap-4 text-xs text-gray-500">
                        <span className="flex items-center gap-1">
                          <span className="font-medium">Type:</span> {rule.ruleType}
                        </span>
                        <span className="flex items-center gap-1">
                          <span className="font-medium">Triggered:</span> {rule.timesTriggered} times
                        </span>
                        {rule.lastTriggeredAt && (
                          <span className="flex items-center gap-1">
                            <span className="font-medium">Last:</span> 
                            {new Date(rule.lastTriggeredAt).toLocaleDateString()}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-8 text-gray-500">
              No active rules found.
            </div>
          )}
        </CardContent>
      </Card>

      {/* Inactive Rules */}
      {inactiveRules.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <XCircle className="h-5 w-5 text-gray-400" />
              Inactive Rules ({inactiveRules.length})
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {inactiveRules.map((rule) => (
                <div
                  key={rule.id}
                  className="border rounded-lg p-4 bg-gray-50 opacity-75"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <Shield className="h-5 w-5 text-gray-400 shrink-0" />
                        <h3 className="font-semibold text-gray-700">{rule.name}</h3>
                        <Badge variant="outline" className="text-xs text-gray-500">
                          Inactive
                        </Badge>
                      </div>
                      
                      <p className="text-sm text-gray-600 mb-3">{rule.description}</p>
                      
                      <div className="flex items-center gap-4 text-xs text-gray-500">
                        <span>Type: {rule.ruleType}</span>
                        <span>Previously triggered: {rule.timesTriggered} times</span>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}