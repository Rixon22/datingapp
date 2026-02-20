import { Component, inject, OnInit, signal } from '@angular/core';import { MembersService } from '../../core/services/members-service';
import { ActivatedRoute } from '@angular/router';
import { Photo } from '../../types/member';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { ImageUpload } from '../../shared/image-upload/image-upload';

@Component({
  selector: 'app-member-photos',
  imports: [ImageUpload],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css'
})
export class MemberPhotos implements OnInit {
  private route = inject(ActivatedRoute);
  protected membersService = inject(MembersService);
  protected photos = signal<Photo[]>([]);
  protected loading = signal(false);

  ngOnInit(): void {
    const memberId = this.route.parent?.snapshot.paramMap.get("id");
    if (memberId) {
     this.membersService.getPhotos(memberId).subscribe({
        next: photos => this.photos.set(photos)
      });
    }
  }

  get photoMocks() {
    return Array.from({ length: 10 }, (_, i) => ({
      url: "./user.jpg"
    }));
  }

  onUploadImage(file: File) {
    this.loading.set(true);
    this.membersService.uploadPhoto(file).subscribe({
      next: photo => {
        this.membersService.editMode.set(false);
        this.loading.set(false);
        this.photos.update(photos => [...photos, photo]);
      },
      error: error => {
        console.log('Error while uploading the image: ', error);
        this.loading.set(false);
      }
    })
  }
}
