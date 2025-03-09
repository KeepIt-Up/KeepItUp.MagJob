# Standardy Kodowania dla .NET i Angular - MagJob

Ten dokument opisuje standardy kodowania dla technologii .NET i Angular używanych w projekcie MagJob.

## Standardy dla .NET

Szczegółowe standardy kodowania dla .NET zostały przeniesione do dokumentacji modułu Identity:
[Standardy Kodowania dla .NET - MagJob Identity](../../src/KeepItUp.MagJob.Identity/docs/dotnet-standards.md)

## Standardy dla Angular

### Struktura Projektu

- **Podejście**: Modułowa struktura oparta na funkcjonalnościach
- **Struktura katalogów**:
  ```
  src/Client/Client.Web/
  ├── app/
  │   ├── core/                # Serwisy, guardy, interceptory używane w całej aplikacji
  │   │   ├── auth/            # Autentykacja i autoryzacja
  │   │   ├── http/            # Interceptory HTTP
  │   │   └── services/        # Serwisy współdzielone
  │   ├── features/            # Moduły funkcjonalne
  │   │   ├── identity/        # Moduł tożsamości
  │   │   ├── schedules/       # Moduł grafików
  │   │   └── work-evidence/   # Moduł ewidencji czasu pracy
  │   ├── shared/              # Komponenty, dyrektywy, pipes współdzielone
  │   │   ├── components/      # Współdzielone komponenty
  │   │   ├── directives/      # Współdzielone dyrektywy
  │   │   └── pipes/           # Współdzielone pipes
  │   └── app.module.ts        # Główny moduł aplikacji
  ├── assets/                  # Statyczne zasoby (obrazy, ikony, itp.)
  └── environments/            # Konfiguracje środowiskowe
  ```

### Konwencje Nazewnictwa

- **Pliki**: kebab-case.type.ts
  ```
  user-list.component.ts
  auth.service.ts
  date-format.pipe.ts
  ```
- **Klasy**: PascalCase
  ```typescript
  export class UserListComponent
  export class AuthService
  ```
- **Zmienne, metody**: camelCase
  ```typescript
  const firstName: string;
  getUserById(): Observable<User>
  ```
- **Stałe**: UPPER_SNAKE_CASE lub PascalCase
  ```typescript
  const API_URL = 'https://api.example.com';
  const DefaultPageSize = 10;
  ```
- **Interfejsy**: PascalCase (bez prefiksu I)
  ```typescript
  export interface User
  export interface Organization
  ```
- **Enumy**: PascalCase
  ```typescript
  export enum UserRole
  export enum ScheduleStatus
  ```

### Biblioteki i Frameworki

- **RxJS**: Do reaktywnego programowania i zarządzania asynchronicznością
- **Tailwind CSS**: Do stylowania komponentów
- **Własne komponenty UI**: Zamiast zewnętrznych bibliotek UI

### Zarządzanie Stanem

- **RxJS**: Używanie serwisów z BehaviorSubject/ReplaySubject do zarządzania stanem
- **Przykład**:
  ```typescript
  @Injectable({
    providedIn: 'root'
  })
  export class OrganizationStateService {
    private _organizations = new BehaviorSubject<Organization[]>([]);
    private _selectedOrganization = new BehaviorSubject<Organization | null>(null);
    
    public readonly organizations$ = this._organizations.asObservable();
    public readonly selectedOrganization$ = this._selectedOrganization.asObservable();
    
    constructor(private apiService: ApiService) {}
    
    loadOrganizations(): Observable<Organization[]> {
      return this.apiService.getOrganizations().pipe(
        tap(organizations => this._organizations.next(organizations))
      );
    }
    
    selectOrganization(id: string): void {
      const organization = this._organizations.value.find(org => org.id === id) || null;
      this._selectedOrganization.next(organization);
    }
  }
  ```

### Testowanie

- **Jasmine**: Framework testowy
- **Karma**: Test runner
- **Testy jednostkowe**: Dla serwisów, pipes, komponentów
- **Testy integracyjne**: Dla interakcji między komponentami

### Dobre Praktyki

- **OnPush Change Detection**: Używanie strategii OnPush dla lepszej wydajności
- **Lazy Loading**: Ładowanie modułów na żądanie
- **Reactive Forms**: Używanie Reactive Forms zamiast Template-driven Forms
- **Typowanie**: Ścisłe typowanie z TypeScript
- **Immutability**: Unikanie mutacji stanu, używanie operatorów RxJS do transformacji danych

### Przykład Komponentu

```typescript
import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { OrganizationService } from '../../services/organization.service';
import { Organization } from '../../models/organization.model';

@Component({
  selector: 'app-organization-form',
  templateUrl: './organization-form.component.html',
  styleUrls: ['./organization-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrganizationFormComponent implements OnInit {
  organizationForm: FormGroup;
  isSubmitting = false;
  
  constructor(
    private fb: FormBuilder,
    private organizationService: OrganizationService
  ) {
    this.organizationForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)]
    });
  }
  
  ngOnInit(): void {
  }
  
  onSubmit(): void {
    if (this.organizationForm.invalid || this.isSubmitting) {
      return;
    }
    
    this.isSubmitting = true;
    
    const organization: Partial<Organization> = {
      name: this.organizationForm.get('name')?.value,
      description: this.organizationForm.get('description')?.value
    };
    
    this.organizationService.createOrganization(organization)
      .subscribe({
        next: () => {
          // Handle success
          this.organizationForm.reset();
        },
        error: (error) => {
          // Handle error
          console.error('Error creating organization', error);
        },
        complete: () => {
          this.isSubmitting = false;
        }
      });
  }
}
```

### Przykład Serwisu

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Organization } from '../models/organization.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private apiUrl = `${environment.apiUrl}/organizations`;
  
  constructor(private http: HttpClient) {}
  
  getOrganizations(): Observable<Organization[]> {
    return this.http.get<Organization[]>(this.apiUrl);
  }
  
  getOrganizationById(id: string): Observable<Organization> {
    return this.http.get<Organization>(`${this.apiUrl}/${id}`);
  }
  
  createOrganization(organization: Partial<Organization>): Observable<Organization> {
    return this.http.post<Organization>(this.apiUrl, organization);
  }
  
  updateOrganization(id: string, organization: Partial<Organization>): Observable<Organization> {
    return this.http.put<Organization>(`${this.apiUrl}/${id}`, organization);
  }
  
  deleteOrganization(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
``` 
