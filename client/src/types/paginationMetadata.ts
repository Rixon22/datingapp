export type PaginationMetadata = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type PaginationResult<T> = {
  items: T[];
  metadata: PaginationMetadata;
}