import { Component, inject, OnInit, signal } from '@angular/core';
import { MessagesService } from '../../core/services/messages-service';
import { Message } from '../../types/message';
import { PaginationResult } from '../../types/paginationMetadata';

@Component({
  selector: 'app-messages',
  imports: [],
  templateUrl: './messages.html',
  styleUrl: './messages.css'
})
export class Messages implements OnInit {
  private messagesService = inject(MessagesService);
  protected container = 'inbox';
  protected pageNumber = 1;
  protected pageSize = 10;
  protected paginatedMessages = signal<PaginationResult<Message> | null>(null);

   ngOnInit(): void {
    this.loadMessages()
  }

  loadMessages() {
    this.messagesService.getMessages(this.container, this.pageNumber, this.pageSize).subscribe({
      next: response => this.paginatedMessages.set(response)
    });
  }

}