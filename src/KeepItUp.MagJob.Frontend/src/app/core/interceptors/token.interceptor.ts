import { HttpRequest, HttpHandlerFn, HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { Observable } from 'rxjs';
import { inject } from '@angular/core';
import { environment } from '@environments/environment';
import { AuthService } from '@core/auth/services/auth.service';

export const tokenInterceptorFn: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);

  // Add token only to requests to API
  if (request.url.startsWith(environment.apiUrl)) {
    if (authService.hasValidAccessToken()) {
      request = addToken(request, authService.getAccessToken());
    }
  }

  return next(request);
};

function addToken(request: HttpRequest<unknown>, token: string) {
  const tokenType = 'Bearer';
  return request.clone({
    setHeaders: {
      Authorization: `${tokenType} ${token}`,
    },
  });
}
