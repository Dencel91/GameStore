import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../interfaces/product';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private url = environment.productUrl;

  constructor(private http: HttpClient) { }

  GetProducts(nextPageCursor?: number, pageSize?: number): Observable<any> {
    let params = new HttpParams();

    if (nextPageCursor) {
      params = params.set('nextPageCursor', nextPageCursor.toString());
    }

    if (pageSize) {
      params = params.set('pageSize', pageSize.toString());
    }

    return this.http.get(this.url, { params });
  }

  GetProduct(id: number): Observable<Product> {
    return this.http.get<Product>(this.url + '/' + id);
  }

  GetProductDetails(id: number): Observable<Product> {
    return this.http.get<Product>(this.url + '/GetProductsDetails/' + id);
  }

  SearchProduct(searchText: string): Observable<Product[]> {
    return this.http.get<Product[]>(this.url + '/Search?searchText=' + searchText);
  }

  AddProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(this.url, product);
  }
}
