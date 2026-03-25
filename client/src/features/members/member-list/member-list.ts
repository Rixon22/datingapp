import { Component, inject, OnInit, signal, ViewChild  } from '@angular/core';
import { MembersService } from '../../../core/services/members-service';
import { Member, MemberParams } from '../../../types/member';
import { MemberCard } from "../member-card/member-card";
import { PaginationResult } from '../../../types/paginationMetadata';
import { Paginator } from "../../../shared/paginator/paginator";
import { FilterModal } from '../filter-modal/filter-modal';

@Component({
  selector: 'app-member-list',
  imports: [ MemberCard, Paginator, FilterModal],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css'
})
export class MemberList implements OnInit {
  @ViewChild('filterModal') modal!: FilterModal;
  private membersService = inject(MembersService);
  protected paginatedMembers = signal<PaginationResult<Member> | null>(null);
  protected memberParams = new MemberParams();

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.membersService.getMembers(this.memberParams).subscribe({      next: result => {
        this.paginatedMembers.set(result);
      }
    }) 
   }

  onPageChange(event: { pageNumber: number, pageSize: number }) {
    this.memberParams.pageNumber = event.pageNumber;
    this.memberParams.pageSize = event.pageSize;
    this.loadMembers();  
  }


  openModal() {
    this.modal.open();
  }

  onClose() {
    console.log('Modal closed');
  }

   onFilterChange(data: MemberParams) {
    this.memberParams = data;
    this.loadMembers();
  }

  resetFilters() {
    this.memberParams = new MemberParams();
    this.loadMembers();
  }
}
