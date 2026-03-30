import { Component, ElementRef, input, model, output, ViewChild, viewChild } from '@angular/core';
import { MemberParams } from '../../../types/member';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filter-modal',
  imports: [FormsModule],
  templateUrl: './filter-modal.html',
})
export class FilterModal {
  @ViewChild('filterModal') modalRef!: ElementRef<HTMLDialogElement>;
  memberParams = model(new MemberParams());
  closeModal = output();
  submitData = output<MemberParams>();

  constructor() {
    const filters = localStorage.getItem('filters');
    if (filters) {
      this.memberParams.set(JSON.parse(filters));
    }
  }

  open() {
    this.modalRef.nativeElement.showModal();
  }

  close() {
    this.modalRef.nativeElement.close();
    this.closeModal.emit();
  }

  submit() {
    this.submitData.emit(this.memberParams());
    this.close();
  }

  onMinAgeChange() {
    if (this.memberParams().minAge < 18) this.memberParams().minAge = 18;
  }

  onMaxAgeChange() {
    if (this.memberParams().maxAge < this.memberParams().minAge) {
      this.memberParams().maxAge = this.memberParams().minAge;
    }
  }
}