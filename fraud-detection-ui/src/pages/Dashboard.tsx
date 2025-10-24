export const Dashboard = () => {
  return (
    <div className="p-8">
      <h1 className="text-4xl font-bold text-blue-600 mb-4">
        Fraud Detection Dashboard
      </h1>
      
      <div className="grid grid-cols-3 gap-6 mt-8">
        {/* Card 1 */}
        <div className="bg-white rounded-lg shadow-lg p-6 border-l-4 border-blue-500">
          <h3 className="text-gray-500 text-sm font-semibold uppercase">Total Transactions</h3>
          <p className="text-3xl font-bold text-gray-800 mt-2">1,234</p>
          <p className="text-green-600 text-sm mt-2">↑ 12% from last week</p>
        </div>

        {/* Card 2 */}
        <div className="bg-white rounded-lg shadow-lg p-6 border-l-4 border-red-500">
          <h3 className="text-gray-500 text-sm font-semibold uppercase">Fraud Alerts</h3>
          <p className="text-3xl font-bold text-gray-800 mt-2">23</p>
          <p className="text-red-600 text-sm mt-2">↑ 5% from last week</p>
        </div>

        {/* Card 3 */}
        <div className="bg-white rounded-lg shadow-lg p-6 border-l-4 border-green-500">
          <h3 className="text-gray-500 text-sm font-semibold uppercase">Detection Rate</h3>
          <p className="text-3xl font-bold text-gray-800 mt-2">98.5%</p>
          <p className="text-green-600 text-sm mt-2">↑ 2% from last week</p>
        </div>
      </div>

      {/* Status Section */}
      <div className="mt-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg p-8 text-white">
        <h2 className="text-2xl font-bold">System Status</h2>
        <p className="mt-2 text-blue-100">All systems operational. Real-time monitoring active.</p>
        <button className="mt-4 bg-white text-blue-600 px-6 py-2 rounded-full font-semibold hover:bg-blue-50 transition">
          View Details
        </button>
      </div>

      {/* Alert Box */}
      <div className="mt-8 bg-yellow-50 border-l-4 border-yellow-400 p-4 rounded">
        <div className="flex">
          <div className="shrink-0">
            <svg className="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
            </svg>
          </div>
          <div className="ml-3">
            <p className="text-sm text-yellow-700 font-medium">
              Testing Tailwind CSS
            </p>
            <p className="text-sm text-yellow-600 mt-1">
              If you see colors, shadows, and nice styling, Tailwind is working! 🎉
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};