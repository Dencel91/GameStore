import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const isAuthenticated = this.authService.isAuthenticated();

    if (isAuthenticated) {
      const path = next.routeConfig?.path;
      if (path === 'signup') {
        this.router.navigate(['/']);
        return false;
      }
      return true;
    }

    if (next.routeConfig?.path === 'signup') {
      return true;
    }

    this.router.navigate(['/']);
    return false;
  }
}