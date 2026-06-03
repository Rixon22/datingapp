import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../../types/user';
import { Message } from '../../types/message';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private hubUrl = environment.hubUrl;
  private toast = inject(ToastService);
  hubConnection?: HubConnection;
  onlineUsers = signal<string[]>([]);

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch((error: any) => console.log(error));

    this.hubConnection.on('UserOnline', (userId: any) => {
      this.onlineUsers.update(users => [...users, userId]);
    });

    this.hubConnection.on('UserOffline', (userId: any) => {
      this.onlineUsers.update(users => users.filter(u => u !== userId))
    });

    this.hubConnection.on('GetOnlineUsers', (userIds: any) => {
      this.onlineUsers.set(userIds);
    });

    this.hubConnection.on('NewMessageReceived', (message: Message) => {
      this.toast.info(message.senderDisplayName + ' sent you a message',
        10000,
        message.senderImageUrl,
        `/members/${message.senderId}/messages`);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((error: any) => console.log(error));
    }
  }
}
