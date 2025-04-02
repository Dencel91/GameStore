import { Component, inject, signal, TemplateRef, ViewChild, WritableSignal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ModalDismissReasons, NgbModal, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { LoginModalComponent } from "../login-modal/login-modal.component";
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { CartService } from '../services/cart.service';
import { AdminPanelComponent } from "../admin-panel/admin-panel.component";

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, LoginModalComponent, AdminPanelComponent],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  @ViewChild(LoginModalComponent) loginModal!: LoginModalComponent;
  @ViewChild(AdminPanelComponent) adminPanel!: AdminPanelComponent;
  
  constructor(
    public authService: AuthService,
    public userService: UserService,
    public cartService: CartService) {}

  showLoginModal() {
	  this.loginModal.open();
  }

  logout() {
    this.authService.logout();
  }

  showAdminPanel() {
    this.adminPanel.open();
  }
}
