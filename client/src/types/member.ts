export interface Member {
  id: string;
  birthDay: string;
  imageUrl?: string;
  displayName: string;
  created: string;
  lastActive: string;
  gender: string;
  description?: string;
  city: string;
  country: string;
}

export interface Photo {
  id: number;
  url: string;
  publicId?: string;
  memberId: string;
}

export interface EditableMember {
  displayName: string;
  city: string;
  country: string;
  description: string;
}

export class MemberParams {
  gender?: string;
  minAge = 18;
  maxAge = 120;
  pageNumber = 1;
  pageSize = 10;
  orderBy = 'age';
}