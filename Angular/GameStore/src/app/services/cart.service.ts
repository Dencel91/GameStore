import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Cart } from '../interfaces/cart';
import { catchError, map, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  constructor(private http: HttpClient, private authService: AuthService) {
    this.authService.loginEvent$.subscribe(isLoggedIn => {
      if (isLoggedIn === null) {
        return;
      }
      
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
      }),
      catchError(error => {
        if (error.status === 404) {
          this.removeCart();
        }

        return throwError(() => error);
      })
    );
  }

  AddToCart(productId: Number): Observable<Cart> {
    let body = {
      productId: productId,
      cartId: this.cartId ?? 0
    }

    let requestUrl = this.url + '/add-product';

    return this.http.post<Cart>(requestUrl, body).pipe(
      map((cart: Cart) => {
        this.setCart(cart);
        return cart;
      })
    );
  }

  RemoveFromCart(productId: Number) : Observable<Cart> {
    const options = {body: {cartId: this.cartId, productId}};
    return this.http.delete<Cart>(this.url + '/remove-product', options).pipe(
      map((cart: Cart) => {
        this.setCart(cart);
        return cart;
      }
    ));
  }

  mergeCarts(): Observable<Cart> {
    return this.http.post<Cart>(this.url + '/merge', this.cartId);
  }

  StartPayment(): Observable<any> {
    return this.http.post(this.url + '/payment/start', this.cartId);
  }

  CompletePayment(): Observable<any> {
    return this.http.post(this.url + '/payment/complete', this.cartId).pipe(
      map((response: any) => {
        this.removeCart();
        return response;
      })
    );
  }
}
