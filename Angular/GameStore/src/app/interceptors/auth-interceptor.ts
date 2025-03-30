import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { catchError, Observable, switchMap, throwError } from "rxjs";
import { AuthService } from "../services/auth.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(@Inject(AuthService) private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.token;
    if (token) {
      const cloned = req.clone({
        headers: req.headers.set("Authorization", `Bearer ${token}`)
      });
      return next.handle(cloned).pipe(
        catchError((error) => this.handleAuthError(cloned, next, error))
      );
    } else {
      return next.handle(req).pipe(
        catchError((error) => this.handleAuthError(req, next, error))
      );
    }
  }

  private handleAuthError(req: HttpRequest<any>, next: HttpHandler, error: HttpErrorResponse): Observable<HttpEvent<any>> {
    if (error.status === 401) {
      return this.authService.refreshToken().pipe(
        switchMap((response: { accessToken: string }) => {
          const cloned = req.clone({
            headers: req.headers.set("Authorization", `Bearer ${response.accessToken}`)
          });
          return next.handle(cloned);
        }),
        catchError((refreshError) => {
          if (refreshError.status === 401) {
            this.authService.logout();
          }
          return throwError(() => refreshError);
        })
      );
    } else if (error.status === 403) {
      console.error("Forbidden request", error);
    } else {
      console.error("An error occurred", error);
    }
    return throwError(() => error);
  }
}