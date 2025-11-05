import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_SIGNALR_URL || 'https://localhost:7249/hubs/fraud';

export type SignalRConnectionStatus = 'Connected' | 'Connecting' | 'Disconnected' | 'Reconnecting';

export class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private statusCallbacks: Array<(status: SignalRConnectionStatus) => void> = [];

  async start() {
    if (this.connection) {
      return this.connection;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    this.connection.onreconnecting(() => {
      console.log('SignalR Reconnecting...');
      this.notifyStatus('Reconnecting');
    });

    this.connection.onreconnected(() => {
      console.log('SignalR Reconnected');
      this.notifyStatus('Connected');
    });

    this.connection.onclose(() => {
      console.log('SignalR Connection Closed');
      this.notifyStatus('Disconnected');
    });

    try {
      this.notifyStatus('Connecting');
      await this.connection.start();
      console.log('SignalR Connected');
      this.notifyStatus('Connected');
    } catch (err) {
      console.error('SignalR Connection Error:', err);
      this.notifyStatus('Disconnected');
    }

    return this.connection;
  }

  async stop() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.notifyStatus('Disconnected');
    }
  }

  getConnection() {
    return this.connection;
  }

  getStatus(): SignalRConnectionStatus {
    if (!this.connection) return 'Disconnected';
    
    switch (this.connection.state) {
      case signalR.HubConnectionState.Connected:
        return 'Connected';
      case signalR.HubConnectionState.Connecting:
      case signalR.HubConnectionState.Reconnecting:
        return 'Connecting';
      default:
        return 'Disconnected';
    }
  }

  onStatusChange(callback: (status: SignalRConnectionStatus) => void) {
    this.statusCallbacks.push(callback);
    return () => {
      this.statusCallbacks = this.statusCallbacks.filter(cb => cb !== callback);
    };
  }

  private notifyStatus(status: SignalRConnectionStatus) {
    this.statusCallbacks.forEach(callback => callback(status));
  }

  // Event listeners
  onTransactionCreated(callback: (data: any) => void) {
    this.connection?.on('TransactionCreated', callback);
  }

  onFraudDetected(callback: (data: any) => void) {
    this.connection?.on('FraudDetected', callback);
  }

  onAlertStatusChanged(callback: (data: any) => void) {
    this.connection?.on('AlertStatusChanged', callback);
  }

  // Clean up listeners
  off(eventName: string) {
    this.connection?.off(eventName);
  }
}

export const signalRService = new SignalRService();