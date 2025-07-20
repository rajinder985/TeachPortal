import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { 
  AuthResponse, 
  LoginRequest, 
  RegisterRequest, 
  Teacher, 
  Student, 
  CreateStudentRequest, 
  RegisterResponse,
  PagedResult,
  ApiError 
} from '../types';

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7260/api',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
        return Promise.reject(this.handleError(error));
      }
    );
  }

  private handleError(error: any): ApiError {
    if (error.response) {
      return {
        message: error.response.data?.message || 'An unexpected error occurred',
        statusCode: error.response.status
      };
    }
    return {
      message: error.message || 'Network error occured'
    };
  }

  // Auth endpoints
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.client.post('/auth/login', credentials);
    return response.data;
  }

  async register(data: RegisterRequest): Promise<RegisterResponse> {
    const response: AxiosResponse<RegisterResponse> = await this.client.post('/auth/register', data);
    return response.data;
  }

  // Teacher endpoints
  async getCurrentTeacher(): Promise<Teacher> {
    const response: AxiosResponse<Teacher> = await this.client.get('/teachers/me');
    return response.data;
  }

async getAllTeachers(): Promise<Teacher[]> {
    const response: AxiosResponse<Teacher[]> = await this.client.get('/teachers');
    return response.data;
  }

  // Student endpoints
  async getMyStudents(pageNumber: number = 1, pageSize: number = 10, search?: string): Promise<PagedResult<Student>> {
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });
    
    if (search) {
      params.append('search', search);
    }

    const response: AxiosResponse<PagedResult<Student>> = await this.client.get(`/students?${params}`);
    return response.data;
  }

  async getStudent(id: number): Promise<Student> {
    const response: AxiosResponse<Student> = await this.client.get(`/students/${id}`);
    return response.data;
  }

  async createStudent(data: CreateStudentRequest): Promise<Student> {
    const response: AxiosResponse<Student> = await this.client.post('/students', data);
    return response.data;
  }

  async deleteStudent(id: number): Promise<void> {
    await this.client.delete(`/students/${id}`);
  }
}

export const apiService = new ApiService();
export default apiService;