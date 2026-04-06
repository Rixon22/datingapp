import { Component, computed, inject, input } from '@angular/core';import { Member } from '../../../types/member';
import { RouterLink } from "@angular/router";
import { AgePipe } from '../../../core/pipes/age-pipe';
import { LikesService } from '../../../core/services/likes-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css'
})
export class MemberCard {
  private likesService = inject(LikesService);
  protected hasLiked = computed(() => this.likesService.likedIds().includes(this.member().id));
  member = input.required<Member>();
  
  toggleLike(event: Event) {
    event.stopPropagation();
    this.likesService.toggleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likesService.likedIds.update(ids => ids.filter(i => i !== this.member().id));
        } else {
          this.likesService.likedIds.update(ids => [...ids, this.member().id]);
        }
      }
    });
  }
}
