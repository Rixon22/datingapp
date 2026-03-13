import { HttpClient, HttpParams  } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditableMember, Member, Photo } from '../../types/member';
import { Observable, tap } from 'rxjs';
import { signal } from '@angular/core';
import { PaginationResult } from '../../types/paginationMetadata';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  editMode = signal(false);
  member = signal<Member | null>(null);

  getMember(id: string): Observable<Member> {
    return this.http.get<Member>(this.baseUrl + "members/" + id).pipe(
      tap(member => {
        this.member.set(member);
      })
    );
  }

    getMembers(pageNumber = 1, pageSize = 5): Observable<PaginationResult<Member>> {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);

    return this.http.get<PaginationResult<Member>>(this.baseUrl + "members", { params });
  }

  getPhotos(id: string) {
    return this.http.get<Photo[]>(`${this.baseUrl}members/${id}/photos`);
  }

  updateMember(member: EditableMember) {
    return this.http.put(this.baseUrl + "members", member);
  }

  uploadPhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'members/photo', formData);
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + 'members/photo/' + photo.id, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'members/photo/' + photoId);
  }
}
