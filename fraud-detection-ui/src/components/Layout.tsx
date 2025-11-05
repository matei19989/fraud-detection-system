import type { ReactNode } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { LayoutDashboard, Receipt, AlertTriangle, Shield, Activity } from 'lucide-react';
import { cn } from '../lib/utils';

interface LayoutProps {
  children: ReactNode;
  isSignalRConnected: boolean;
}

const navigation = [
  { name: 'Dashboard', href: '/', icon: LayoutDashboard },
  { name: 'Transactions', href: '/transactions', icon: Receipt },
  { name: 'Alerts', href: '/alerts', icon: AlertTriangle },
  { name: 'Rules', href: '/rules', icon: Shield },
];

export function Layout({ children, isSignalRConnected }: LayoutProps) {
  const location = useLocation();

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Sidebar */}
      <div className="fixed inset-y-0 left-0 z-50 w-64 bg-white border-r border-gray-200">
        {/* Logo */}
        <div className="flex h-16 items-center gap-2 px-6 border-b border-gray-200">
          <Shield className="h-8 w-8 text-blue-600" />
          <div>
            <h1 className="text-lg font-bold text-gray-900">FraudShield</h1>
            <p className="text-xs text-gray-500">Detection System</p>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 space-y-1 px-3 py-4">
          {navigation.map((item) => {
            const isActive = location.pathname === item.href;
            const Icon = item.icon;

            return (
              <Link
                key={item.name}
                to={item.href}
                className={cn(
                  'group flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                  isActive
                    ? 'bg-blue-50 text-blue-700'
                    : 'text-gray-700 hover:bg-gray-100 hover:text-gray-900'
                )}
              >
                <Icon
                  className={cn(
                    'h-5 w-5',
                    isActive ? 'text-blue-700' : 'text-gray-400 group-hover:text-gray-600'
                  )}
                />
                {item.name}
              </Link>
            );
          })}
        </nav>

        {/* SignalR Status */}
        <div className="absolute bottom-0 left-0 right-0 border-t border-gray-200 bg-white p-4">
          <div className="flex items-center gap-2 text-sm">
            <div className={cn(
              'h-2 w-2 rounded-full',
              isSignalRConnected ? 'bg-green-500' : 'bg-red-500'
            )} />
            <span className="text-gray-600">
              {isSignalRConnected ? 'Connected' : 'Disconnected'}
            </span>
            <Activity className={cn(
              'h-4 w-4 ml-auto',
              isSignalRConnected ? 'text-green-500 animate-pulse' : 'text-gray-400'
            )} />
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="pl-64">
        <main className="py-8 px-8">
          {children}
        </main>
      </div>
    </div>
  );
}