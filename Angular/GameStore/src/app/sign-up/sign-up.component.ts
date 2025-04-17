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

  get username() {
    return this.signUpForm.get('userName');
  }
  
  get email() {
    return this.signUpForm.get('email');
  }

  get password() {
    return this.signUpForm.get('password');
  }

  get checkPassword() {
    return this.signUpForm.get('checkPassword');
  }

  get agreed() {
    return this.signUpForm.get('agreed');
  }

  signUpForm = new FormGroup({
    userName: new FormControl<string>('', [Validators.required]),
    email: new FormControl<string>('', [Validators.required, Validators.email]),
    password: new FormControl<string>('', [Validators.required, Validators.minLength(5)]),
    checkPassword: new FormControl<string>('', [Validators.required, Validators.minLength(5)]),
    agreed: new FormControl(false, [Validators.requiredTrue]),
  });

  submit() {
    if (!this.signUpForm.valid) {
      this.signUpForm.markAllAsTouched();
      return;
    }

    this.authService.register(
      this.signUpForm.value.userName!,
      this.signUpForm.value.email!,
      this.signUpForm.value.password!,
      this.signUpForm.value.checkPassword!)
      .subscribe(() => {
        this.router.navigate(['/login']);
      });
  };
}
