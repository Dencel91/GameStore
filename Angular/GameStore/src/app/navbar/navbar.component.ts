import { Component, inject, signal, TemplateRef, ViewChild, WritableSignal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ModalDismissReasons, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LoginModalComponent } from "../login-modal/login-modal.component";
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, LoginModalComponent],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  @ViewChild(LoginModalComponent) loginModal!: LoginModalComponent;
  
  constructor(public authService: AuthService, public userService: UserService) {}

  showLoginModal() {
	  this.loginModal.open();
  }

  logout() {
    this.authService.logout();
  }
}
