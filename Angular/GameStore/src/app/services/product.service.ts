import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../interfaces/product';
import { environment } from '../../environments/environment';
import { FileHandler } from '../interfaces/fileHandler';

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

  addProduct(product: any): Observable<Product> {
    const formData = new FormData();
    formData.append('name', product.name);
    formData.append('description', product.description);
    formData.append('price', product.price);
    formData.append('thumbnail', product.thumbnail.file);
    product.images.forEach((image: FileHandler) => {
      formData.append('images', image.file);
    });

    return this.http.post<Product>(this.url, formData);
  }

  UpdateProduct(product: any): Observable<Product> {
    const formData = new FormData();
    formData.append('productId', product.id);
    formData.append('name', product.name);
    formData.append('description', product.description);
    formData.append('price', product.price);
    if (product.updatedThumbnail) {
      formData.append('UpdatedThumbnail', product.updatedThumbnail.file);
    }
    product.newImages.forEach((image: FileHandler) => {
      formData.append('newImages', image.file);
    });

    product.removedImages.forEach((url: string, index: number) => {
      formData.append(`removedImages[${index}]`, url);
    });

    return this.http.patch<Product>(this.url, formData);
  }
}
