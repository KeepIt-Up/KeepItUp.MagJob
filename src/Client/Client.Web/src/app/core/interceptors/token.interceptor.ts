import { HttpRequest, HttpHandlerFn, HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { Observable } from 'rxjs';
import { inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

export const tokenInterceptor: HttpInterceptorFn = (request: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
    const authService = inject(OAuthService);

    if (authService.hasValidAccessToken()) {
        request = addToken(request, authService.getAccessToken()!);
    }

    return next(request);
};

function addToken(request: HttpRequest<any>, token: string) {
    const tokenType = 'Bearer';
    return request.clone({
        setHeaders: {
            'Authorization': `${tokenType} ${token}`
        }
    });
}
