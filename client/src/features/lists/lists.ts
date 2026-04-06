import { Component, inject, OnInit, signal } from '@angular/core';
import { LikesService } from '../../core/services/likes-service';
import { Member } from '../../types/member';

@Component({
  selector: 'app-lists',
  imports: [],
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})

export class Lists implements OnInit {
  private likesService = inject(LikesService);
  private members = signal<Member[]>([]);
  protected predicate = 'liked';

  tabs = [
    { label: 'Liked', value: 'liked'},
    { label: 'Liked me', value: 'likedby'},
    { label: 'Mutual', value: 'mutual'}
  ];

  ngOnInit(): void {
    this.loadLikes();
  }

  setPredicate(predicate: string) {
    if (this.predicate !== predicate) {
      this.predicate = predicate;
      this.loadLikes();
    }
  }

  loadLikes() {
    this.likesService.getLikes(this.predicate).subscribe({
      next: members => this.members.set(members)
    });
  }
}
