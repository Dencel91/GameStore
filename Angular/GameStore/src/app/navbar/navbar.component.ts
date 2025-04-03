import { Component, ViewChild } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { LoginModalComponent } from "../login-modal/login-modal.component";
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { CartService } from '../services/cart.service';
import { AdminPanelComponent } from "../admin-panel/admin-panel.component";
import { ProductService } from '../services/product.service';

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
    public cartService: CartService,
    public productService: ProductService,
    private router: Router) { }

  showLoginModal() {
	  this.loginModal.open();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  showAdminPanel() {
    this.adminPanel.open();
  }

  searchProduct(term: string) {
    if (!term) {
      return;
    }

    this.router.navigate(['/search', term]);
  }
}
