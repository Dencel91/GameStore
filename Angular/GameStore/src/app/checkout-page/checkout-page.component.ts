import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CartService } from '../services/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-checkout-page',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './checkout-page.component.html',
  styleUrl: './checkout-page.component.css'
})
export class CheckoutPageComponent {
  constructor(private cartService: CartService, private router: Router) { }

  paymentMethod: PaymentMethod = PaymentMethod.None;

  get cartNumber() {
    return this.creditCardForm.get('cardNumber');
  }

  creditCardForm = new FormGroup({
    cardNumber: new FormControl<string>('', [Validators.required, Validators.pattern(/^\d{4} \d{4} \d{4} \d{4}$/)]),
    month: new FormControl<string>('', [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])$/)]),
    year: new FormControl<string>('', [Validators.required]),
    cvc: new FormControl<string>('', [Validators.required]),
    ownerName: new FormControl<string>('', [Validators.required]),
  });

  submit() {
    switch (this.paymentMethod) {
      case PaymentMethod.Card:
        this.payByCreditCard();
        break;
      default:
        console.error('unexpected payment type');
        break;
    }
  }

  payByCreditCard() {
    if (!this.creditCardForm.valid) {
      this.creditCardForm.markAllAsTouched();
      return;
    }

    // Here should be some third-party payment service integration
    const success = true; // Simulate payment success
    if (success) {
      this.completePayment();
    } else {
      console.error("Payment failed");
    }
  }

  completePayment() {
    this.cartService.CompletePayment().subscribe((response: any) => {
      console.log("Payment completed successfully", response);
      this.router.navigate(['/library']);
    });
  }

  onCardInput(event: any) {
    let input = event.target.value.replace(/\D/g, ''); // Remove non-digits
    input = input.substring(0, 16); // Limit to 16 digits
    this.creditCardForm.patchValue({ cardNumber: input.replace(/(.{4})/g, '$1 ').trim() }); // Add spaces every 4 digits
  }

  onMonthInput(event: any) {
    let input = event.target.value.replace(/\D/g, ''); // Remove non-digits
    this.creditCardForm.patchValue({ month: input.trim() });

    this.validateExpiryDate();
  }

  onYearInput(event: any) {
    let input = event.target.value.replace(/\D/g, ''); // Remove non-digits
    this.creditCardForm.patchValue({ year: input.trim() });

    this.validateExpiryDate();
  }

  onCvcInput(event:any) {
    let input = event.target.value.replace(/\D/g, ''); // Remove non-digits
    this.creditCardForm.patchValue({ cvc: input.trim() });
  }

  validateExpiryDate() {
    const month = this.creditCardForm.value.month;
    const year = this.creditCardForm.value.year;

    if (month?.length == 2 && year?.length == 2) {

      const yearNumber = parseInt(year);
      const monthNumber = parseInt(month);
      let now = new Date();

      var currentYear = now.getFullYear() % 100;
      var currentMonth = now.getMonth();

      if ((yearNumber < currentYear) || (yearNumber == currentYear && monthNumber < currentMonth)) {
        this.creditCardForm.get('month')?.setErrors({ invalidDate: true });
        this.creditCardForm.get('year')?.setErrors({ invalidDate: true });
      } else {
        this.creditCardForm.get('month')?.setErrors(null);
        this.creditCardForm.get('year')?.setErrors(null);
      }
    }
  }
}

enum PaymentMethod {
  None = 0,
  Card = 1
}
