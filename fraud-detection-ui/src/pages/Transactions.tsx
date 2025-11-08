import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { transactionsApi } from '../services/api';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Button } from '../components/ui/button';
import { formatCurrency, formatDateLong, getRiskColor } from '../lib/utils';
import { Search, Filter } from 'lucide-react';

export function Transactions() {
  const [filters, setFilters] = useState({
    status: '',
    riskLevel: '',
    accountId: '',
  });
  const [searchInput, setSearchInput] = useState('');

  const { data: transactions, isLoading } = useQuery({
    queryKey: ['transactions', filters],
    queryFn: () => transactionsApi.getAll({
      status: filters.status || undefined,
      riskLevel: filters.riskLevel || undefined,
      accountId: filters.accountId || undefined,
      pageSize: 50,
    }).then(res => res.data),
    refetchInterval: 5000,
  });

  const handleSearch = () => {
    setFilters({ ...filters, accountId: searchInput });
  };

  const handleClearFilters = () => {
    setFilters({ status: '', riskLevel: '', accountId: '' });
    setSearchInput('');
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight text-gray-900">Transactions</h2>
        <p className="text-gray-500 mt-1">Monitor and analyze all transactions</p>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg flex items-center gap-2">
            <Filter className="h-5 w-5" />
            Filters
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap gap-4">
            {/* Search */}
            <div className="flex gap-2 flex-1 min-w-[200px]">
              <input
                type="text"
                placeholder="Search by Account ID..."
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                className="flex-1 px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <Button onClick={handleSearch} size="sm">
                <Search className="h-4 w-4" />
              </Button>
            </div>

            {/* Status Filter */}
            <select
              value={filters.status}
              onChange={(e) => setFilters({ ...filters, status: e.target.value })}
              className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">All Statuses</option>
              <option value="Pending">Pending</option>
              <option value="Approved">Approved</option>
              <option value="Declined">Declined</option>
              <option value="UnderReview">Under Review</option>
            </select>

            {/* Risk Level Filter */}
            <select
              value={filters.riskLevel}
              onChange={(e) => setFilters({ ...filters, riskLevel: e.target.value })}
              className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">All Risk Levels</option>
              <option value="None">None</option>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>

            {/* Clear Filters */}
            {(filters.status || filters.riskLevel || filters.accountId) && (
              <Button variant="outline" onClick={handleClearFilters} size="sm">
                Clear Filters
              </Button>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Transactions List */}
      <Card>
        <CardHeader>
          <CardTitle>
            {transactions?.length ?? 0} Transactions
          </CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8 text-gray-500">Loading transactions...</div>
          ) : transactions && transactions.length > 0 ? (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b">
                  <tr>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Account</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Merchant</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Amount</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Risk</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Score</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {transactions.map((tx) => (
                    <tr key={tx.id} className="hover:bg-gray-50">
                      <td className="px-4 py-3 text-sm font-mono text-gray-600">
                        {tx.id.substring(0, 8)}...
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-900">{tx.accountId}</td>
                      <td className="px-4 py-3">
                        <div>
                          <div className="text-sm font-medium text-gray-900">{tx.merchantName}</div>
                          <div className="text-xs text-gray-500">{tx.merchantCategory}</div>
                        </div>
                      </td>
                      <td className="px-4 py-3 text-sm font-semibold text-gray-900">
                        {formatCurrency(tx.amount, tx.currency)}
                      </td>
                      <td className="px-4 py-3">
                        <Badge variant="outline" className="text-xs">
                          {tx.status}
                        </Badge>
                      </td>
                      <td className="px-4 py-3">
                        <Badge className={getRiskColor(tx.riskLevel)}>
                          {tx.riskLevel}
                        </Badge>
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-900">
                        {tx.fraudScore.toFixed(1)}
                      </td>
                      <td className="px-4 py-3 text-xs text-gray-500">
                        {formatDateLong(tx.createdAt)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="text-center py-8 text-gray-500">
              No transactions found. Try adjusting your filters.
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}