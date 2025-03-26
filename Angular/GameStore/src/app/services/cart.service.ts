import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Product } from '../interfaces/product';
import { Cart } from '../interfaces/cart';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  // private url = 'https://dencel.com/api/carts';
  private url = 'https://localhost:7055/api/carts';


  get cartId(): number{
    return parseInt(localStorage.getItem("cartId") ?? "0");
  }

  set cartId(value: number){
    localStorage.setItem("cartId", value.toString());
  }

  constructor(private http: HttpClient) { }

  GetCart(): Observable<Cart> {
    return this.http.get<Cart>(this.url + "/" + this.cartId);
  }

  AddToCart(productId: Number): Observable<Cart> {
    let body = {
      productId: productId,
      cartId: this.cartId ?? 0
    }

    let requestUrl = this.url + '/AddProduct';

    return this.http.post<Cart>(requestUrl, body).pipe(
      map((cart: Cart) => {
        if(!this.cartId)
          {
            this.cartId = cart.id;
            console.log("Set cart id", this.cartId);
          }
    
          console.log("Product added to cart", body);

          return cart;
      })
    );
  }

  RemoveFromCart(productId: Number) : Observable<Cart> {
    const options = {body: {cartId: this.cartId, productId}};
    return this.http.delete<Cart>(this.url + '/RemoveProduct', options).pipe(
      map((cart: Cart) => {
        console.log("Product removed from cart", productId, cart);
        return cart;
      }
    ));
  }

  StartPayment() {
    this.http.post(this.url + '/payment', null);

    console.log("Payment started");
  }

  CompletePayment() {
    this.http.post(this.url + '/payment/complete', null);

    console.log("Payment completed");
  }
}
