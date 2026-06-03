import { Component, effect, ElementRef, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MessagesService } from '../../core/services/messages-service';
import { MembersService } from '../../core/services/members-service';
import { Message } from '../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';
import { PresenceService } from '../../core/services/presence-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit, OnDestroy {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;
  protected messagesService = inject(MessagesService);
  private membersService = inject(MembersService);
  private router = inject(ActivatedRoute);
  protected messageContent = '';
  protected presenceService = inject(PresenceService);

  constructor() {
    effect(() => {
      const currentMessages = this.messagesService.messageThread();
      if (currentMessages.length > 0) {
        this.scrollToBottom();
      }
    });
  }

  ngOnInit(): void {
    this.router.parent?.paramMap.subscribe({
      next: params => {
        const otherUserId = params.get('id');
        if (!otherUserId) throw new Error('Cannot connect to hub');
        this.messagesService.createHubConnection(otherUserId);
      }
    })
  }

  sendMessage() {
    const recipientId = this.membersService.member()?.id;
    if (!recipientId) return;
    this.messagesService.sendMessage(recipientId, this.messageContent)?.then(() => {
      this.messageContent = '';
    });
  }

  scrollToBottom() {
    setTimeout(() => {
      if (this.messageEndRef) {
        this.messageEndRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
      }
    });
  }

  ngOnDestroy(): void {
    this.messagesService.stopHubConnection();
  }
}
