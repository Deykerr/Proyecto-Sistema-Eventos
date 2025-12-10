export interface CategoryRequest {
  name: string;
  description: string;
}

export interface Category {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
}