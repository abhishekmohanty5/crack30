export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
  batch: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface RefreshResponse {
  accessToken: string;
  refreshToken: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
  role: 'Student' | 'Admin';
  batch: string;
  targetCompany?: string;
  interviewDate?: string;
}

export interface UpdateProfileRequest {
  name?: string;
  batch?: string;
  targetCompany?: string;
  interviewDate?: string;
}
