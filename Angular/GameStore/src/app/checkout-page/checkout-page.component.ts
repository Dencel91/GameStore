import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CartService } from '../services/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-checkout-page',
  imports: [FormsModule],
  templateUrl: './checkout-page.component.html',
  styleUrl: './checkout-page.component.css'
})
export class CheckoutPageComponent {
  constructor(private cartService: CartService, private router: Router) { }

  paymentMethod: number = 0;

  validateDate() {

  }

  completePayment() {

    // Here should be some third-party payment service integration
    const success = true; // Simulate payment success
    if (success) {
      this.cartService.CompletePayment().subscribe((response: any) => {
        console.log("Payment completed successfully", response);
        this.router.navigate(['/library']);
      });
    } else {
      console.error("Payment failed");
    }
  }
}
