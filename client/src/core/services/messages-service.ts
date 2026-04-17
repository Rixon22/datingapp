import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginationResult } from '../../types/paginationMetadata';
import { Message } from '../../types/message';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getMessages(container: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    params = params.append('container', container);

    return this.http.get<PaginationResult<Message>>(this.baseUrl + 'messages', {params});
  }
}