import { Component } from '@angular/core';
import { CartService } from '../services/cart.service';
import { Cart } from '../interfaces/cart';
import { CartItemComponent } from "../cart-item/cart-item.component";
import { Router } from '@angular/router';
import { LoadingComponent } from "../loading/loading.component";

@Component({
  selector: 'app-cart-page',
  imports: [CartItemComponent, LoadingComponent],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.css'
})
export class CartPageComponent {
  constructor(private cartService: CartService, private router: Router) { }

  cart: Cart | undefined;
  loading: boolean = true;

  ngOnInit() {
    console.log('Cart page loaded');

    if (this.cartService.cartId) {
      this.cartService.GetCart().subscribe((data: Cart) => {
        console.log('Cart items:', data);
        this.cart = data;
        this.loading = false;
      });
    }
    else {
      this.loading = false;
    }
  }

  handleCartUpdated(cart: Cart) {
    console.log("handleCartUpdated");
    this.cart = cart;
  }

  startPayment() {
    this.cartService.StartPayment().subscribe((response: any) => {
      this.router.navigate(['/checkout']);
    });
  }
}
