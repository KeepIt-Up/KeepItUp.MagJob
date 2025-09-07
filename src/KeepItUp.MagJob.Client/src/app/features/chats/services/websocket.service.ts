import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { Client, Message, StompSubscription } from '@stomp/stompjs';
import SockJS from 'sockjs-client';

export interface WebSocketMessage {
  type: string;
  payload: unknown;
}

@Injectable({
  providedIn: 'root',
})
export class WebSocketService {
  private client: Client | null = null;
  private messageSubject = new Subject<WebSocketMessage>();
  private connectionSubject = new Subject<boolean>();
  private subscriptions = new Map<string, StompSubscription>();

  constructor() {}

  connect(url: string): Promise<void> {
    return new Promise((resolve, reject) => {
      try {
        this.client = new Client({
          webSocketFactory: () => new SockJS(url),
          debug: str => {
            console.log('STOMP Debug:', str);
          },
          reconnectDelay: 5000,
          heartbeatIncoming: 4000,
          heartbeatOutgoing: 4000,
        });

        this.client.onConnect = () => {
          console.log('WebSocket connected');
          this.connectionSubject.next(true);
          resolve();
        };

        this.client.onStompError = frame => {
          console.error('STOMP error:', frame);
          this.connectionSubject.next(false);
          reject(new Error(frame.headers['message'] ?? 'STOMP connection error'));
        };

        this.client.onWebSocketError = error => {
          console.error('WebSocket error:', error);
          this.connectionSubject.next(false);
          reject(new Error(error instanceof Error ? error.message : String(error)));
        };

        this.client.onWebSocketClose = () => {
          console.log('WebSocket disconnected');
          this.connectionSubject.next(false);
        };

        this.client.activate();
      } catch (error) {
        reject(new Error(error instanceof Error ? error.message : 'Connection error'));
      }
    });
  }

  subscribe(topic: string): Observable<Message> {
    if (!this.client?.connected) {
      throw new Error('WebSocket not connected');
    }

    if (this.subscriptions.has(topic)) {
      console.log('Already subscribed to topic:', topic);
      throw new Error(`Already subscribed to topic: ${topic}`);
    }

    const messageSubject = new Subject<Message>();

    const subscription = this.client.subscribe(topic, message => {
      messageSubject.next(message);
    });

    this.subscriptions.set(topic, subscription);

    return messageSubject.asObservable();
  }

  send(destination: string, message: unknown): void {
    if (!this.client?.connected) {
      throw new Error('WebSocket not connected');
    }

    this.client.publish({
      destination,
      body: JSON.stringify(message),
    });
  }

  unsubscribe(topic: string): void {
    const subscription = this.subscriptions.get(topic);
    if (subscription) {
      subscription.unsubscribe();
      this.subscriptions.delete(topic);
    }
  }

  disconnect(): void {
    if (this.client) {
      this.subscriptions.forEach(subscription => {
        subscription.unsubscribe();
      });
      this.subscriptions.clear();

      void this.client.deactivate();
      this.client = null;
      this.connectionSubject.next(false);
    }
  }

  isConnected(): boolean {
    return this.client?.connected ?? false;
  }

  onConnectionChange(): Observable<boolean> {
    return this.connectionSubject.asObservable();
  }

  onMessage(): Observable<WebSocketMessage> {
    return this.messageSubject.asObservable();
  }
}
