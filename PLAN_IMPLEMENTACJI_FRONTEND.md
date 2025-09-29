# Plan Implementacji KeepItUp.MagJob.Frontend

## Cel Projektu

Stworzenie nowej aplikacji Frontend wykorzystującej PrimeNG przy zachowaniu spójności z istniejącą aplikacją Client oraz implementowaniu najlepszych praktyk Angular v19.

## Analiza Obecnego Stanu

### KeepItUp.MagJob.Client (Stara Aplikacja)

- **UI Framework**: Tailwind CSS + ng-icons
- **Architektura**: Standalone Components, Feature-based structure
- **Layout**: Navbar z dark mode, responsive design
- **Strony**: Landing, Help, Not-Found, Unauthorized
- **Funkcjonalności**: Autentykacja, Dark mode, Profile dropdown, Mobile menu

### KeepItUp.MagJob.Frontend (Nowa Aplikacja)

- **UI Framework**: PrimeNG v19.1.3, PrimeFlex, PrimeIcons
- **Angular**: v19.2.0 (najnowsza wersja)
- **Architektura**: Feature-based z anonymous/authenticated layouts
- **Status**: Podstawowa struktura, komponenty w trakcie implementacji

## Faza 1: Przygotowanie Publicznego Layout

### 1.1 Implementacja Anonymous Layout z PrimeNG

**Komponenty do utworzenia/aktualizacji:**

#### `anonymous-layout.component.ts`

```typescript
// Wykorzystanie PrimeNG Toolbar, Menubar
// Integracja z AuthService
// Responsive design z PrimeFlex
```

#### `topbar.component.ts` (aktualizacja)

**Funkcjonalności:**

- Logo MagJob (typography z PrimeNG)
- Navigation menu (PrimeNG Menubar)
- Dark mode toggle (PrimeNG Button + ToggleButton)
- User authentication buttons (PrimeNG Button)
- Profile dropdown (PrimeNG Menu)
- Mobile hamburger menu (PrimeNG Sidebar)

**Styling:**

- Wykorzystanie PrimeNG Theme system
- PrimeFlex dla responsive layout
- Zachowanie kolorystyki: biały/szary background, primary colors

#### `footer.component.ts`

**Funkcjonalności:**

- Copyright information
- Links (help, terms, privacy)
- Social media icons (PrimeIcons)

**Styling:**

- PrimeNG Panel lub Card dla struktury
- PrimeFlex grid system

## Faza 2: Implementacja Kluczowych Stron

### 2.1 Landing Page

#### `landing.component.ts`

**Sekcje:**

1. **Hero Section**

   - PrimeNG Card z gradientowym background
   - Typography (h1, p) z PrimeNG
   - Call-to-action buttons (PrimeNG Button)
   - Illustration/hero image

2. **Features Section**

   - PrimeNG DataView lub grid z Cards
   - Feature cards z ikonami (PrimeIcons)
   - Animacje hover (PrimeNG Ripple)

3. **CTA Section**
   - PrimeNG Panel z highlighted background
   - Centered content z PrimeFlex

**Komponenty PrimeNG:**

- `p-card` dla sekcji
- `p-button` dla akcji
- `p-divider` dla separacji
- PrimeFlex dla layoutu

### 2.2 Help Page

#### `help.component.ts`

**Funkcjonalności:**

- FAQ sections (PrimeNG Accordion)
- Search functionality (PrimeNG InputText + AutoComplete)
- Categories (PrimeNG TabView)
- Contact form (PrimeNG form components)

**Sekcje:**

1. **Search Section**

   - PrimeNG InputGroup z search icon
   - Quick links (PrimeNG Chip)

2. **FAQ Categories**

   - PrimeNG TabView dla kategorii
   - PrimeNG Accordion dla pytań

3. **Contact Support**
   - PrimeNG Panel z contact form
   - Validation z PrimeNG Message

### 2.3 Not Found Page (404)

#### `not-found.component.ts`

**Elementy:**

- Error illustration
- Typography (PrimeNG)
- Navigation buttons (PrimeNG Button)
- Suggested links (PrimeNG ListBox)

**Layout:**

- Centered content z PrimeFlex
- Error code styling
- Breadcrumb navigation (PrimeNG Breadcrumb)

### 2.4 Unauthorized Page (403)

#### `unauthorized.component.ts`

**Elementy:**

- Access denied message
- Login/Register buttons (PrimeNG Button)
- Back to home link
- Security illustration

## Faza 3: Implementacja Wspólnych Komponentów

### 3.1 Theme Service

#### `theme.service.ts`

```typescript
// Dark/Light mode switching
// PrimeNG theme integration
// LocalStorage persistence
// System preference detection
```

### 3.2 Layout Components

#### `loading.component.ts`

- PrimeNG ProgressSpinner
- Overlay z PrimeNG OverlayPanel

#### `error-boundary.component.ts`

- PrimeNG Message dla błędów
- PrimeNG Dialog dla error modals

### 3.3 Navigation Components

#### `breadcrumb.component.ts`

- PrimeNG Breadcrumb
- Reactive route-based generation

#### `page-header.component.ts`

- Standardized page headers
- PrimeNG Typography + Divider

## Faza 4: Routing i Navigation

### 4.1 Route Configuration

#### `app.routes.ts`

```typescript
// Anonymous routes
// Authenticated routes
// Route guards
// Lazy loading
```

### 4.2 Navigation Guards

#### `auth.guard.ts`

- Integration z AuthService
- Redirect logic
- PrimeNG Toast dla notifications

## Faza 5: State Management i Services

### 5.1 Authentication Integration

#### `auth.service.ts`

- OAuth2 OIDC integration
- Token management
- User context

### 5.2 Layout State Service

#### `layout.service.ts`

- Mobile menu state
- Sidebar state
- Theme preferences

## Faza 6: Responsive Design i Accessibility

### 6.1 Responsive Breakpoints

- Mobile-first approach z PrimeFlex
- Breakpoints: xs, sm, md, lg, xl
- Component adaptations

### 6.2 Accessibility (A11Y)

- ARIA labels
- Keyboard navigation
- Screen reader support
- Color contrast compliance

## Faza 7: Testing i Quality Assurance

### 7.1 Unit Tests

- Component testing z Jasmine/Karma
- Service testing
- Mock strategies

### 7.2 E2E Tests

- User journey testing
- Cross-browser compatibility
- Mobile device testing

## Faza 8: Performance i Optimization

### 8.1 Bundle Optimization

- Tree shaking
- Lazy loading
- Code splitting

### 8.2 Performance Monitoring

- Core Web Vitals
- Bundle analysis
- Performance budgets

## Timeline Implementacji

### Sprint 1 (Tydzień 1-2): Layout Foundation

- [ ] Anonymous layout implementation
- [ ] Topbar z PrimeNG
- [ ] Footer component
- [ ] Theme service
- [ ] Basic routing

### Sprint 2 (Tydzień 3-4): Core Pages

- [ ] Landing page implementation
- [ ] Help page implementation
- [ ] Not-found page
- [ ] Unauthorized page

### Sprint 3 (Tydzień 5-6): Polish & Integration

- [ ] Authentication integration
- [ ] Error handling
- [ ] Loading states
- [ ] Mobile responsiveness

### Sprint 4 (Tydzień 7-8): Testing & Optimization

- [ ] Unit tests
- [ ] E2E tests
- [ ] Performance optimization
- [ ] Accessibility audit

## Kluczowe Decyzje Techniczne

### PrimeNG Components Selection

- **Layout**: Toolbar, Menubar, Sidebar
- **Navigation**: Breadcrumb, Steps, TabView
- **Data Display**: Card, DataView, Panel
- **Forms**: InputText, Button, Dropdown
- **Feedback**: Message, Toast, ProgressSpinner
- **Overlay**: Dialog, Menu, Tooltip

### Styling Approach

- PrimeNG themes jako base
- Custom CSS variables dla branding
- PrimeFlex dla layout utilities
- Component-specific overrides w SCSS

### State Management

- Angular Signals dla reactive state
- Services dla business logic
- RxJS dla async operations
- Local storage dla preferences

## Wytyczne Implementacyjne

### Code Quality

- ESLint configuration zgodna z Angular v19
- Prettier dla formatowania
- Strict TypeScript settings
- Angular best practices (OnPush, trackBy, etc.)

### Component Architecture

- Standalone components
- Smart/Dumb component pattern
- Input/Output dla communication
- Dependency injection

### Performance Considerations

- OnPush change detection
- Lazy loading routes
- Image optimization
- Bundle size monitoring

## Migracja z Client App

### Nie kopiować bezpośrednio:

- Tailwind classes → PrimeNG components
- Custom components → PrimeNG equivalents
- Utility classes → PrimeFlex utilities

### Zachować:

- Color scheme i branding
- User experience patterns
- Navigation structure
- Content hierarchy

### Ulepszyć:

- Component reusability
- Accessibility
- Performance
- Maintainability

## Risyki i Mitigation

### Potencjalne Problemy:

1. **Theme consistency** - Regular visual reviews
2. **Performance regression** - Bundle size monitoring
3. **PrimeNG learning curve** - Team training sessions
4. **Responsive breakpoints** - Device testing matrix

### Success Metrics:

- Bundle size ≤ 2MB initial
- First Contentful Paint ≤ 2s
- Lighthouse score ≥ 90
- Zero accessibility violations
- 100% component test coverage

---

_Ten plan jest żywy dokument i będzie aktualizowany w trakcie implementacji na podstawie learnings i feedback._
