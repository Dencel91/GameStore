import { Component } from '@angular/core';
import { CartService } from '../services/cart.service';
import { Cart } from '../interfaces/cart';
import { CartItemComponent } from "../cart-item/cart-item.component";

@Component({
  selector: 'app-cart-page',
  imports: [CartItemComponent],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.css'
})
export class CartPageComponent {
  cart: Cart | undefined;

  constructor(private cartService: CartService) { }

  ngOnInit() {
    console.log('Cart page loaded');

    if (this.cartService.cartId) {
      this.cartService.GetCart().subscribe((data: Cart) => {
        console.log('Cart items:', data);
        this.cart = data;
      });
    }
  }

  handleCartUpdated(cart: Cart) {
    console.log("handleCartUpdated");
    this.cart = cart;
  }

  startPayment() {
    console.log('Payment started');
  }
}
