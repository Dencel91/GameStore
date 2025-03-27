import { Component, inject, signal, TemplateRef, ViewChild, WritableSignal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ModalDismissReasons, NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../services/auth.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-login-modal',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './login-modal.component.html',
  styleUrl: './login-modal.component.css'
})
export class LoginModalComponent {
  @ViewChild('content', { static: true }) content!: TemplateRef<any>;
  private modalService = inject(NgbModal);
  modal: NgbModalRef | undefined;
  closeResult: WritableSignal<string> = signal('');

  loginForm = new FormGroup({
    userName: new FormControl(),
    password: new FormControl(),
  });
  

  constructor(private authService: AuthService) { }

  open() {
		this.modal = this.modalService.open(this.content, { ariaLabelledBy: 'modal-basic-title' });
    
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
