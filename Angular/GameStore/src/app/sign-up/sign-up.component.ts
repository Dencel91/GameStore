import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-up',
  imports: [ReactiveFormsModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {

  constructor(private authService: AuthService, private router: Router) { }

  signUpForm = new FormGroup({
    userName: new FormControl(),
    email: new FormControl(),
    password: new FormControl(),
    checkPassword: new FormControl(),
    // email: new FormControl(),
    agreed: new FormControl(false, [Validators.requiredTrue]),
  });

  submit() {
    this.authService.register(
      this.signUpForm.value.userName,
      this.signUpForm.value.email,
      this.signUpForm.value.password,
      this.signUpForm.value.checkPassword)
      .subscribe(() => {
        this.router.navigate(['/login']);
      });
  };
}
