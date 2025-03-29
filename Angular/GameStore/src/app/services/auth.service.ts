import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserService } from './user.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  url = environment.authUrl;

  isLoggedIn = this.refreshToken !== '';

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
