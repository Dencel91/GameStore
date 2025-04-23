import { Component, inject, signal, TemplateRef, ViewChild, WritableSignal } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal, NgbModalRef, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { CredentialResponse } from 'google-one-tap';
import { AuthService } from '../services/auth.service';
import { Router, RouterLink } from '@angular/router';
declare const google: any;

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent {
  constructor(private router: Router, private authService: AuthService) { }

  googleClientId: string = '956787144564-1b0cbsa63v7anv2qo2c6fn5rf8ev8i6r.apps.googleusercontent.com';
  error: string = '';

  loginForm = new FormGroup({
    userName: new FormControl<string>('', [Validators.required]),
    password: new FormControl<string>('', [Validators.required]),
  });

  get username() {
    return this.loginForm.get("userName");
  }

  get password() {
    return this.loginForm.get("password");
  }
  
  ngOnInit() {
    this.initializeGoogleSignIn();
  }

  initializeGoogleSignIn() {
    google.accounts.id.initialize({
      client_id: this.googleClientId,
      
      callback: this.handleCredentialResponse.bind(this),
    });

    google.accounts.id.prompt();

    this.triggerGoogleSignIn();
  }

  triggerGoogleSignIn() {
    google.accounts.id.renderButton(
      document.getElementById("googleSignIn") as HTMLElement,
      { theme: "filled_black", text: "continue_with", size: "large", width: "400px" }
    );
  }

  handleCredentialResponse(response: CredentialResponse) {
    this.authService.googleLogin(response.credential).subscribe((response: any) => {
      this.router.navigate(['/store']);
    });
  }

  login() {
    if (!this.loginForm.valid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.authService.login(this.loginForm.value.userName!, this.loginForm.value.password!).subscribe({
      next:(() => {
        this.router.navigate(['/store']);
      }),
      error: ((error) => {
        if (error.status === 400 && typeof error.error.detail === 'string') {
          this.error = error.error.detail;
        }
        else {
          console.error(error);
        }
      })
    });
  }
}
