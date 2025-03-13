import { HttpRequest, HttpHandlerFn, HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { Observable } from 'rxjs';
import { inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { environment } from '@environments/environment';

export const tokenInterceptor: HttpInterceptorFn = (request: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
    const oauthService = inject(OAuthService);

    // Dodaj token tylko do żądań do naszego API
    if (request.url.startsWith(environment.apiUrl)) {
        if (oauthService.hasValidAccessToken()) {
            request = addToken(request, oauthService.getAccessToken()!);
        }
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
