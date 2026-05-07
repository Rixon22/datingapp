import { Component, effect, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MessagesService } from '../../core/services/messages-service';
import { MembersService } from '../../core/services/members-service';
import { Message } from '../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit{
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;
  private messagesService = inject(MessagesService);
  private membersService = inject(MembersService);
  protected messages = signal<Message[]>([]);
  protected messageContent = '';

  constructor() {
    effect(() => {
      const currentMessages = this.messages();
      if (currentMessages.length > 0) {
        this.scrollToBottom();
      }
    });
  }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    const memberId = this.membersService.member()?.id;
    if (memberId) {
      this.messagesService.getMessageThread(memberId).subscribe({
        next: messages => this.messages.set(messages.map(message => ({
          ...message,
          currentUserSender: message.senderId !== memberId
        })))
      })
    }
  }

  sendMessage() {
    const recipientId = this.membersService.member()?.id;
    if (!recipientId) return;
    this.messagesService.sendMessage(recipientId, this.messageContent).subscribe({
      next: message => {
        this.messages.update(messages => {
          message.currentUserSender = true;
          return [...messages, message];
        });
        this.messageContent = '';
      }
    })
  }

  scrollToBottom() {
    setTimeout(() => {
      if (this.messageEndRef) {
        this.messageEndRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
      }
    });
  }
}
