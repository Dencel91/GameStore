import { Component } from '@angular/core';
import { CartService } from '../services/cart.service';
import { Cart } from '../interfaces/cart';
import { CartItemComponent } from "../cart-item/cart-item.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart-page',
  imports: [CartItemComponent],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.css'
})
export class CartPageComponent {
  cart: Cart | undefined;

  constructor(private cartService: CartService, private router: Router) { }

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
    this.cartService.StartPayment().subscribe((response: any) => {
      this.router.navigate(['/checkout']);
    });
  }
}
