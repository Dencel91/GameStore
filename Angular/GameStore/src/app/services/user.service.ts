import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Product } from '../interfaces/product';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  url = environment.userUrl;

  set user(value: any) {
    localStorage.setItem('user', JSON.stringify(value));
  }

  get user(): any {
    return JSON.parse(localStorage.getItem('user') ?? '{}');
  }

  constructor(private httpClient: HttpClient) { }

  getUserInfo() {
    const options = {
      headers: new HttpHeaders({
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      })
    };

    this.httpClient.get(this.url, options).subscribe((response: any) => {
      this.user = response;
    });
  }

  getUserProducts() : Observable<Product[]> {
    return this.httpClient.get<Product[]>(this.url + '/products');
  }
}
