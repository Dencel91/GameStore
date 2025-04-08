import { Component, inject, signal, TemplateRef, ViewChild, WritableSignal } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
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

  googleClientId: string = '1025391897494-kcsdhm4qvkftht1amto3h4qtd1vh0tmk.apps.googleusercontent.com';

  @ViewChild('content', { static: true }) content!: TemplateRef<any>;
  private modalService = inject(NgbModal);
  modal: NgbModalRef | undefined;
  closeResult: WritableSignal<string> = signal('');

  loginForm = new FormGroup({
    userName: new FormControl(),
    password: new FormControl(),
  });
  
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
    google.accounts.id.prompt((notification: any) => {
      google.accounts.id.renderButton(
        document.getElementById("googleSignIn") as HTMLElement,
        { theme: "filled_black", text: "continue_with", size: "large", width: "400px" }
      );
    });
  }

  handleCredentialResponse(response: CredentialResponse) {
    this.authService.googleLogin(response.credential).subscribe((response: any) => {
      this.router.navigate(['/store']);
    });
  }

  open() {
		this.modal = this.modalService.open(this.content, { ariaLabelledBy: 'modal-basic-title' });
    
    this.triggerGoogleSignIn();

    this.modal.result.then(
			(result) => {
				this.closeResult.set(`Closed with: ${result}`);
			},
			(reason) => {
				this.closeResult.set(`Dismissed ${this.getDismissReason(reason)}`);
			},
		);
	}

  private getDismissReason(reason: any): string {
		switch (reason) {
			case ModalDismissReasons.ESC:
				return 'by pressing ESC';
			case ModalDismissReasons.BACKDROP_CLICK:
				return 'by clicking on a backdrop';
			default:
				return `with: ${reason}`;
		}
	}

  login() {
    this.authService.login(this.loginForm.value.userName, this.loginForm.value.password);
    this.modal?.close();
  }
}
