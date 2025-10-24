import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_SIGNALR_URL || 'https://localhost:7001/fraudHub';

let connection: signalR.HubConnection | null = null;

export const startSignalRConnection = async () => {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL)
    .withAutomaticReconnect()
    .build();

  try {
    await connection.start();
    console.log('SignalR Connected');
  } catch (err) {
    console.error('SignalR Connection Error: ', err);
  }

  return connection;
};

export const getConnection = () => connection;