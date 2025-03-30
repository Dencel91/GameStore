import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserService } from './user.service';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  url = environment.authUrl;

  isLoggedIn = this.getRefreshToken() !== '';

  get token(): string {
    return localStorage.getItem('token') ?? '';
  }

  set token(value: string) {
    localStorage.setItem('token', value);
  }

  getRefreshToken(): string {
    return localStorage.getItem('refreshToken') ?? '';
  }

  setRefreshToken(value: string) {
    localStorage.setItem('refreshToken', value);
    this.isLoggedIn = value !== '';
  }

  isAuthenticated(): boolean {
    return this.isLoggedIn;
  }


  constructor(private http: HttpClient, private userService: UserService) { }

  login(username: string, password: string) {
    this.http.post(this.url + '/login', {username, password}).subscribe((response: any) => {
      this.token = response.accessToken;
      this.setRefreshToken(response.refreshToken);

      this.userService.getUserInfo();
    });
  }

  refreshToken() : Observable<any> {
    const request = {
      userId: this.userService.user.id,
      RefreshToken: this.getRefreshToken()
    };

    return this.http.post(this.url + '/refresh-token', request).pipe(
      map((response: any) => {
        this.token = response.accessToken;
        this.setRefreshToken(response.refreshToken);
        return response;
      }));
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
    this.setRefreshToken('');
    this.userService.user = {};
  }
}
