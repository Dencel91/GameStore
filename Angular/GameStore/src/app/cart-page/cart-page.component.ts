import { Component } from '@angular/core';
import { CartService } from '../services/cart.service';
import { Cart } from '../interfaces/cart';

@Component({
  selector: 'app-cart-page',
  imports: [],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.css'
})
export class CartPageComponent {
  cart: Cart = {} as Cart;

  constructor(private cartService: CartService) { }

  ngOnInit() {
    console.log('Cart page loaded');

    this.cartService.GetCart().subscribe((data: Cart) => {
      console.log('Cart items:', data);
      this.cart = data;
    });
  }

  removeFromCart(productId: Number) {
    this.cartService.RemoveFromCart(productId).subscribe({
      next: (data: Cart) => {
        this.cart = data;
        console.log('Product removed from cart:', data);
      }, error: (error: any) => {
        console.error(error);
      }
    });
  }

  startPayment() {
    console.log('Payment started');
  }

  completePayment() {
    console.log('Payment completed');
  }
}
