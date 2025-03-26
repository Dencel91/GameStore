import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../interfaces/product';
import { CartService } from '../services/cart.service';
import { Cart } from '../interfaces/cart';

@Component({
  selector: 'app-cart-item',
  imports: [],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.css'
})
export class CartItemComponent {
  @Input() product: Product = {} as Product;
  @Output() cartUpdated = new EventEmitter();

  constructor(private cartService: CartService) {}

  removeFromCart(productId: Number) {
    this.cartService.RemoveFromCart(productId).subscribe({
      next: (cart: Cart) => {
        this.cartUpdated.emit(cart);
      }, error: (error: any) => {
        console.error(error);
      }
    });
  }
}
