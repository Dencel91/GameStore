import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Product } from '../interfaces/product';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient, private authService: AuthService) { 
    this.authService.loginEvent$.subscribe(isLoggedIn => {
      if (isLoggedIn) {
        this.getUserInfo();
      } else {
        localStorage.removeItem('user');
      }
    });
  }

  url = environment.userUrl;

  set user(value: any) {
    localStorage.setItem('user', JSON.stringify(value));
  }

  get user(): any {
    return JSON.parse(localStorage.getItem('user') ?? '{}');
  }

  getUserInfo() {
    this.httpClient.get(this.url).subscribe((response: any) => {
      this.user = response;
    });
  }

  getUserProducts() : Observable<Product[]> {
    return this.httpClient.get<Product[]>(this.url + '/products');
  }

  getUserProductInfo(productId: number): Observable<any> {
    const headers = new HttpHeaders({ 'No-Redirect': 'true' });

    const options = {headers};
    return this.httpClient.get(this.url + '/products/' + productId, options);
  }

  addFreeProductToUser(productId: number): Observable<any> {
    return this.httpClient.post(this.url + '/products', productId);
  }
}
