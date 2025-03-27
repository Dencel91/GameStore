import { HttpClient } from '@angular/common/http';
import { computed, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  isLoggedIn = this.refreshToken !== '';
  // isLoggedIn = computed(() => );

  get token(): string {
    return localStorage.getItem('token') ?? '';
  }

  set token(value: string) {
    localStorage.setItem('token', value);
  }

  get refreshToken(): string {
    return localStorage.getItem('refreshToken') ?? '';
  }

  set refreshToken(value: string) {
    localStorage.setItem('refreshToken', value);
    this.isLoggedIn = this.refreshToken !== '';
  }

  url = 'https://localhost:7208/api/auth';

  constructor(private http: HttpClient, private userService: UserService) { }

  login(username: string, password: string) {
    this.http.post(this.url + '/login', {username, password}).subscribe((response: any) => {
      this.token = response.accessToken;
      this.refreshToken = response.refreshToken;

      this.userService.getUserInfo();
    });
  }

  register(username: string, password: string, verifyPassword: string) {

    const request = {
      username: username,
      password: password,
      verifypassword: verifyPassword
    };

    return this.http.post(this.url + '/register', request);
  }

  logout() {
    this.token = '';
    this.refreshToken = '';
    this.userService.user = {};
  }
}
