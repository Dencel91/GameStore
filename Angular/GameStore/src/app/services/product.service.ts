import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../interfaces/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  //private url = 'https://localhost:7243/api/products';
  private url = 'https://localhost:7243/api/products';

  constructor(private http: HttpClient) { }

  GetProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.url);
  }

  GetProduct(id: number): Observable<Product> {
    return this.http.get<Product>(this.url + '/' + id);
  }

  GetProductDetails(id: number): Observable<Product> {
    return this.http.get<Product>(this.url + '/GetProductsDetails/' + id);
  }

  AddProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(this.url, product);
  }
}
