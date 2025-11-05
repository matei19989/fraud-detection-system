import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_SIGNALR_URL || 'http://localhost:5121/hubs/fraud';

export type SignalRConnectionStatus = 'Connected' | 'Connecting' | 'Disconnected' | 'Reconnecting';

export class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private statusCallbacks: Array<(status: SignalRConnectionStatus) => void> = [];

  /** Start or resume the SignalR connection */
  async start(): Promise<signalR.HubConnection | null> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      return this.connection;
    }

    // Create a new connection if needed
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, { transport: signalR.HttpTransportType.WebSockets })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Register lifecycle events
    this.connection.onreconnecting(() => this.notifyStatus('Reconnecting'));
    this.connection.onreconnected(() => this.notifyStatus('Connected'));
    this.connection.onclose(() => this.notifyStatus('Disconnected'));

    // Immediately notify 'Connecting'
    this.notifyStatus('Connecting');

    try {
      await this.connection.start();
      console.log('SignalR Connected');
      this.notifyStatus('Connected');
      return this.connection;
    } catch (err: any) {
      console.error('SignalR failed to start:', err);
      this.notifyStatus('Disconnected');
      return null;
    }
  }

  /** Stop the SignalR connection */
  async stop() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.notifyStatus('Disconnected');
    }
  }

  /** Get current connection instance */
  getConnection() {
    return this.connection;
  }

  /** Get current connection status based on the HubConnection state */
  getStatus(): SignalRConnectionStatus {
  if (!this.connection) return 'Disconnected';

  switch (this.connection.state) {
    case signalR.HubConnectionState.Connected:
      return 'Connected';
    case signalR.HubConnectionState.Connecting:
    case signalR.HubConnectionState.Reconnecting:
      return 'Connected'; // treat these as "connected" for UX
    default:
      return 'Disconnected';
  }
}


  /** Subscribe to status changes */
  onStatusChange(callback: (status: SignalRConnectionStatus) => void) {
    this.statusCallbacks.push(callback);
    // Immediately notify current status
    callback(this.getStatus());
    return () => {
      this.statusCallbacks = this.statusCallbacks.filter(cb => cb !== callback);
    };
  }

  /** Notify all listeners about a status change */
  private notifyStatus(status: SignalRConnectionStatus) {
    this.statusCallbacks.forEach(cb => cb(status));
  }

  /** Event listeners */
  onTransactionCreated(callback: (data: any) => void) {
    this.connection?.on('TransactionCreated', callback);
  }

  onFraudDetected(callback: (data: any) => void) {
    this.connection?.on('FraudDetected', callback);
  }

  onAlertStatusChanged(callback: (data: any) => void) {
    this.connection?.on('AlertStatusChanged', callback);
  }

  off(eventName: string) {
    this.connection?.off(eventName);
  }

  /** Debug: log all incoming messages for testing */
  debugAllEvents() {
    if (!this.connection) return;
    const originalOn = this.connection.on.bind(this.connection);
    this.connection.on = (methodName, newMethod) => {
      originalOn(methodName, (data) => {
        console.log(`[SignalR Event] ${methodName}:`, data);
        newMethod(data);
      });
    };
  }
}

// Singleton instance
export const signalRService = new SignalRService();
