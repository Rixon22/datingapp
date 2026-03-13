import { Component, inject } from '@angular/core';
import { MembersService } from '../../../core/services/members-service';
import { Member } from '../../../types/member';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { MemberCard } from "../member-card/member-card";
import { PaginationResult } from '../../../types/paginationMetadata';

@Component({
  selector: 'app-member-list',
  imports: [AsyncPipe, MemberCard],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})
export class MemberList {
  private membersService = inject(MembersService);
  protected paginatedMembers$: Observable<PaginationResult<Member>>;

  constructor() {
    this.paginatedMembers$ = this.membersService.getMembers();
  }
}
