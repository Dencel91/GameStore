import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Cart } from '../interfaces/cart';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  constructor(private http: HttpClient, private authService: AuthService) {
    this.authService.loginEvent$.subscribe(isLoggedIn => {
      if (isLoggedIn) {
        if (this.cartId) {
          this.mergeCarts().subscribe(cart => {
            this.setCart(cart);
          });
        }
        else {
          this.GetCurrentUserCart().subscribe(cart => {
            if (cart) {
              this.setCart(cart);
            }
          });
        }
      }
      else {
        this.removeCart();
      }
    });
  }

  private url = environment.cartUrl;

  setCart(cart: Cart) {
    localStorage.setItem("cartId", cart.id.toString());
    localStorage.setItem("cartItemCount", cart.products.length.toString());
  }

  removeCart() {
    localStorage.removeItem("cartId");
    localStorage.removeItem("cartItemCount");
  }

  get cartId(): number{
    return parseInt(localStorage.getItem("cartId") ?? "0");
  }

  set cartId(value: number){
    localStorage.setItem("cartId", value.toString());
  }

  get cartItemCount(): number {
    return parseInt(localStorage.getItem("cartItemCount") ?? "0");
  }

  GetCurrentUserCart(): Observable<Cart> {
    return this.http.get<Cart>(this.url).pipe(
      map((cart: Cart) => {
        if (cart) {
          this.setCart(cart);
        }
        return cart;
      })
    );
  }

  GetCart(): Observable<Cart> {
    return this.http.get<Cart>(this.url + "/" + this.cartId).pipe(
      map((cart: Cart) => {
        this.setCart(cart);
        return cart;
      })
    );
  }

  AddToCart(productId: Number): Observable<Cart> {
    let body = {
      productId: productId,
      cartId: this.cartId ?? 0
    }

    let requestUrl = this.url + '/AddProduct';

    return this.http.post<Cart>(requestUrl, body).pipe(
      map((cart: Cart) => {
        // if(!this.cartId)
        //   {
        //     this.cartId = cart.id;
        //     console.log("Set cart id", this.cartId);
        //   }
        this.setCart(cart);
    
        return cart;
      })
    );
  }

  RemoveFromCart(productId: Number) : Observable<Cart> {
    const options = {body: {cartId: this.cartId, productId}};
    return this.http.delete<Cart>(this.url + '/RemoveProduct', options).pipe(
      map((cart: Cart) => {
        this.setCart(cart);
        return cart;
      }
    ));
  }

  mergeCarts() : Observable<Cart> {
    return this.http.post<Cart>(this.url + '/merge-carts', this.cartId);
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
