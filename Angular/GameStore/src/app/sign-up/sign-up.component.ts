import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-sign-up',
  imports: [ReactiveFormsModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {

  signUpForm = new FormGroup({
    userName: new FormControl(),
    password: new FormControl(),
    checkPassword: new FormControl(),
    email: new FormControl(),
    agreed: new FormControl(false, [Validators.requiredTrue]),
  });

  submit() {
    console.log(this.signUpForm.value);
  }
}
