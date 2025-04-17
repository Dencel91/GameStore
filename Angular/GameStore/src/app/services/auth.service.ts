import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { CustomJwtPayload, NameClaim, RoleClaim, UserIdClaim } from '../interfaces/customJwtPayload';
declare const google: any;

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http: HttpClient) { }

  url = environment.authUrl;

  isLoggedIn = this.getRefreshToken() !== '';

  get token(): string {
    return localStorage.getItem('token') ?? '';
  }

  get userId(): string {
    return localStorage.getItem('userId') ?? '';
  }

  get userName(): string {
    return localStorage.getItem('userName') ?? '';
  }

  getRefreshToken(): string {
    return localStorage.getItem('refreshToken') ?? '';
  }

  setAuthInfo(loginResponse: any) {
    localStorage.setItem('token', loginResponse.accessToken);
    localStorage.setItem('refreshToken', loginResponse.refreshToken);

    
    let decodedToken = this.getDecodedToken();

    if (decodedToken) {
      localStorage.setItem('userId', decodedToken[UserIdClaim]);
      localStorage.setItem('userName', decodedToken[NameClaim]);
    }

    this.isLoggedIn = true;
  }

  removeAuthInfo() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    this.isLoggedIn = false;
  }

  isAuthenticated(): boolean {
    return this.isLoggedIn;
  }

  private loginSubject = new BehaviorSubject<boolean>(false);
  public loginEvent$ = this.loginSubject.asObservable();

  login(username: string, password: string): Observable<any> {
    return this.http.post(this.url + '/login', { username, password }).pipe(
      map((response: any) => {
        this.setAuthInfo(response);
        this.loginSubject.next(true);

        return response;
      })
    );
  }

  googleLogin(credential: string): Observable<any> {
    const headers = { 'Content-Type': 'application/json'}

    const body = JSON.stringify(credential);

    return this.http.post(this.url + '/google-login', body, { headers }).pipe(
      map((response: any) => {
        this.setAuthInfo(response);
        this.loginSubject.next(true);
        return response;
      }));
  }

  refreshToken() : Observable<any> {
    const request = {
      userId: this.userId,
      RefreshToken: this.getRefreshToken()
    };

    return this.http.post(this.url + '/refresh-token', request).pipe(
      map((response: any) => {
        this.setAuthInfo(response);
        return response;
      }));
  }

  register(username: string, email: string, password: string, verifyPassword: string) {

    const request = {
      username: username,
      email: email,
      password: password,
      verifypassword: verifyPassword
    };

    return this.http.post(this.url + '/register', request);
  }

  logout() {
    this.removeAuthInfo();
    google.accounts.id.disableAutoSelect();
    this.loginSubject.next(false);
  }

  isAdmin(): boolean {
    let test = this.getUserRole();
    return this.getUserRole() === 'Admin';
  }

  private getUserName(): string {
    const decodedToken = this.getDecodedToken();
    return decodedToken ? decodedToken[NameClaim] : '';
  }

  private getUserRole(): string {
    const decodedToken = this.getDecodedToken();
    return decodedToken ? decodedToken[RoleClaim] : '';
  }

  private getDecodedToken(): CustomJwtPayload | null {
    try {
      return jwtDecode<CustomJwtPayload>(this.token);
    } catch (error) {
      console.error('Invalid token', error);
      return null;
    }
  }
}
