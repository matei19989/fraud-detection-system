import { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from './components/ui/toaster';
import { signalRService } from './services/signalr';
import { useToast } from './hooks/use-toast';
import { Layout } from './components/Layout';
import { Dashboard } from './pages/Dashboard';
import { Transactions } from './pages/Transactions';
import { Alerts } from './pages/Alerts';
import { Rules } from './pages/Rules';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

function AppContent() {
  const [isSignalRConnected, setIsSignalRConnected] = useState(false);
  const { toast } = useToast();

  useEffect(() => {
    // Start SignalR connection
    signalRService.start().then(() => {
      setIsSignalRConnected(true);

      // Listen for fraud detected events
      signalRService.onFraudDetected((data) => {
        if (data.fraudScore >= 80) {
          toast({
            variant: 'destructive',
            title: '🚨 High-Risk Transaction Detected!',
            description: `Transaction ${data.transactionId.substring(0, 8)}... | Score: ${data.fraudScore.toFixed(1)} | ${data.riskLevel}`,
          });
        }
      });

      // Listen for transaction created events (optional)
      signalRService.onTransactionCreated(() => {
        // Could refresh transactions list here
        queryClient.invalidateQueries({ queryKey: ['transactions'] });
        queryClient.invalidateQueries({ queryKey: ['dashboard'] });
      });

      // Listen for alert status changes
      signalRService.onAlertStatusChanged(() => {
        queryClient.invalidateQueries({ queryKey: ['alerts'] });
        queryClient.invalidateQueries({ queryKey: ['dashboard'] });
      });
    });

    return () => {
      signalRService.stop();
    };
  }, [toast]);

  return (
    <Layout isSignalRConnected={isSignalRConnected}>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/transactions" element={<Transactions />} />
        <Route path="/alerts" element={<Alerts />} />
        <Route path="/rules" element={<Rules />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  );
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AppContent />
        <Toaster />
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;