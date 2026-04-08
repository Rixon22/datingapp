import { Component, HostListener, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { EditableMember, Member } from '../../types/member';
import { DatePipe } from '@angular/common';
import { MembersService } from '../../core/services/members-service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastService } from '../../core/services/toast-service';
import { AccountService } from '../../core/services/account-service';
import { TimeAgoPipe } from '../../core/pipes/time-ago-pipe';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule, TimeAgoPipe],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css'
})
export class MemberProfile implements OnInit, OnDestroy {
  @ViewChild('memberProfileEditForm') memberProfileEditForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify ($event:BeforeUnloadEvent) {
    if (this.memberProfileEditForm?.dirty) {
      $event.preventDefault();
    }
  };
  private accountService = inject(AccountService);
  private toast = inject(ToastService);
  protected membersService = inject(MembersService);
  protected editableMember: EditableMember = {
    displayName: '',
    description: '',
    city: '',
    country: ''
  };

  ngOnInit(): void {
    this.editableMember = {
      displayName: this.membersService.member()?.displayName || '',
      description: this.membersService.member()?.description || '',
      city: this.membersService.member()?.city || '',
      country: this.membersService.member()?.country || ''
    };
  }

  ngOnDestroy(): void {
    if (this.membersService.editMode()) {
      this.membersService.editMode.set(false);
    }
  }

  updateProfile() {
    if (!this.membersService.member()) return;
    const updatedMember = {...this.membersService.member(), ...this.editableMember};
    this.membersService.updateMember(this.editableMember).subscribe({
      next: () => {
        const currentUser = this.accountService.currentUser();
        if (currentUser && updatedMember.displayName !== currentUser?.displayName) {
          currentUser.displayName = updatedMember.displayName;
          this.accountService.setCurrentUser(currentUser);
        }
        this.membersService.editMode.set(false);
        this.membersService.member.set(updatedMember as Member);
        this.memberProfileEditForm?.reset(updatedMember);
        this.toast.success('Profile updated successfully');
      }
    });
  }
}