export interface Teacher {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  createdAt: string;
  lastLoginAt?: string;
  studentCount: number;
}

export interface Student {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  fullName: string;
  createdAt: string;
  teacherId: string;
  teacherName: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  teacher: Teacher;
}

export type RegisterResponse = Teacher;//TODO : Need to update in the main code

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface RegisterRequest {
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  confirmPassword: string;
}

export interface CreateStudentRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

export interface ApiError {
  message: string;
  statusCode?: number;
}