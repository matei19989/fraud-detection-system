import { useEffect, useState, useRef } from 'react';
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
  const initialized = useRef(false);

  useEffect(() => {
    // Prevent double init on HMR
    if (initialized.current) return;
    initialized.current = true;

    let isMounted = true;

    const init = async () => {
      try {
        const conn = await signalRService.start();
        if (!isMounted || !conn) return;

        setIsSignalRConnected(true);
        console.log('SignalR Connected (App)');

        interface FraudData{
          transactionId: string;
          fraudScore: number;
          riskLevel: string;
        }

        // Attach listeners once
        signalRService.onFraudDetected((data : unknown) => {
          const fraudData = data as FraudData;

          if (fraudData.fraudScore >= 80) {
            toast({
              variant: 'destructive',
              title: 'High-Risk Transaction Detected!',
              description: `Transaction ${fraudData.transactionId.substring(0, 8)}... | Score: ${fraudData.fraudScore.toFixed(1)} | ${fraudData.riskLevel}`,
            });
          }
        });

        signalRService.onTransactionCreated(() => {
          queryClient.invalidateQueries({ queryKey: ['transactions'] });
          queryClient.invalidateQueries({ queryKey: ['dashboard'] });
        });

        signalRService.onAlertStatusChanged(() => {
          queryClient.invalidateQueries({ queryKey: ['alerts'] });
          queryClient.invalidateQueries({ queryKey: ['dashboard'] });
        });
      } catch (err) {
        if (isMounted) {
          console.error('SignalR init failed:', err);
          setIsSignalRConnected(false);
        }
      }
    };

    init();

    const unsubscribe = signalRService.onStatusChange((status) => {
      setIsSignalRConnected(status === 'Connected');
    });

    return () => {
      isMounted = false;
      unsubscribe();
      // Don't stop on unmount during dev — let HMR reuse
      if (import.meta.env.PROD) {
        signalRService.stop();
      }
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