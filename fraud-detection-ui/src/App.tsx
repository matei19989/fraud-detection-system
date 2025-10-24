import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { Dashboard } from './pages/Dashboard';
import { Transactions } from './pages/Transactions';
import { Alerts } from './pages/Alerts';

function App() {
  return (
    <BrowserRouter>
      <div className="min-h-screen bg-gray-50">
        <nav className="bg-white shadow-sm border-b">
          <div className="container mx-auto px-4">
            <div className="flex space-x-8 py-4">
              <Link 
                to="/" 
                className="text-blue-600 hover:text-blue-800 font-medium transition"
              >
                Dashboard
              </Link>
              <Link 
                to="/transactions" 
                className="text-blue-600 hover:text-blue-800 font-medium transition"
              >
                Transactions
              </Link>
              <Link 
                to="/alerts" 
                className="text-blue-600 hover:text-blue-800 font-medium transition"
              >
                Alerts
              </Link>
            </div>
          </div>
        </nav>
        
        <main className="container mx-auto">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/transactions" element={<Transactions />} />
            <Route path="/alerts" element={<Alerts />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;