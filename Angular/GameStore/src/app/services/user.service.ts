import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  url = "https://localhost:7153/api/users";

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
}
