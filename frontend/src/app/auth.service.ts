import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE_URL } from './tokens';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private base = inject(API_BASE_URL);

  async login(username: string, password: string) {
    const res = await this.http.post<{ token: string }>(`${this.base}/auth/login`, { username, password }).toPromise();
    if (res?.token) {
      localStorage.setItem('token', res.token);
    }
    return res;
  }

  get token() {
    return localStorage.getItem('token');
  }

  logout() {
    localStorage.removeItem('token');
  }

  get isLoggedIn() {
    return !!this.token;
  }
}
