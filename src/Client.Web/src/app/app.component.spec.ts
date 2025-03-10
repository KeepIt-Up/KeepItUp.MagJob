import { TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AppComponent } from './app.component';
import { UserService } from '@features/services/user.service';
import { DateTimeProvider, OAuthLogger, OAuthService, UrlHelperService } from 'angular-oauth2-oidc';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';
import { NotificationsComponent } from '@shared/components/notifications/notifications.component';
import { BehaviorSubject } from 'rxjs';

describe('AppComponent', () => {
  let userService: UserService;
  let userStateSubject: BehaviorSubject<any>;

  beforeEach(async () => {
    userStateSubject = new BehaviorSubject({ data: null });

    const mockUserService = {
      userState$: () => userStateSubject.value
    };

    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([]),
        AppComponent,
        SpinnerComponent,
        NotificationsComponent
      ],
      providers: [
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
        { provide: UserService, useValue: mockUserService },
        OAuthService,
        OAuthLogger,
        DateTimeProvider,
        UrlHelperService
      ]
    }).compileComponents();

    userService = TestBed.inject(UserService);
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should show spinner when user is not loaded', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();

    const spinnerElement = fixture.nativeElement.querySelector('app-spinner');
    const routerOutlet = fixture.nativeElement.querySelector('router-outlet');

    expect(spinnerElement).toBeTruthy();
    expect(routerOutlet).toBeFalsy();
  });

  it('should show router-outlet when user is loaded', () => {
    userStateSubject.next({ data: { id: '1', name: 'Test User' } });

    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();

    const spinnerElement = fixture.nativeElement.querySelector('app-spinner');
    const routerOutlet = fixture.nativeElement.querySelector('router-outlet');

    expect(spinnerElement).toBeFalsy();
    expect(routerOutlet).toBeTruthy();
  });

  it('should always show notifications component', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();

    const notificationsElement = fixture.nativeElement.querySelector('app-notifications');
    expect(notificationsElement).toBeTruthy();
  });
});
