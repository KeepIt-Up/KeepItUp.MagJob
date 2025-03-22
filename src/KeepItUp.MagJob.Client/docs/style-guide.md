# Style Guide

This style guide outlines the coding conventions and styling practices for the MagJob client application.

## Table of Contents

- [CSS and Styling](#css-and-styling)
  - [TailwindCSS](#tailwindcss)
  - [Component Styling](#component-styling)
- [TypeScript Conventions](#typescript-conventions)
- [HTML Templates](#html-templates)
- [Angular Practices](#angular-practices)
- [Code Formatting](#code-formatting)
- [Documentation](#documentation)
- [Visual Design](#visual-design)

## CSS and Styling

### TailwindCSS

The project uses TailwindCSS for styling. Tailwind provides utility classes that are applied directly in the HTML markup.

```html
<button class="mg-btn-primary">Submit</button>
```

### Component Styling

Components should encapsulate their own styles within their component files. This promotes reusability and maintainability:

```typescript
// user-card.component.ts
@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss'],
})
export class UserCardComponent {}
```

For component-specific styles, use the component's own SCSS file:

```scss
// user-card.component.scss
.user-card {
  @apply rounded-lg shadow-md p-4;

  &__header {
    @apply font-bold text-lg mb-2;
  }

  &__content {
    @apply text-gray-700;
  }
}
```

### Shared Components

Reusable UI components should be placed in the `shared/components` directory. These components should be designed to be flexible and configurable through inputs and outputs:

```
shared/
└── components/
    ├── button/
    ├── card/
    ├── modal/
    └── form-controls/
```

Example of a shared button component:

```typescript
// shared/components/button/button.component.ts
@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
})
export class ButtonComponent {
  @Input() variant: 'primary' | 'outline' | 'text' = 'primary';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() disabled = false;
  @Output() clicked = new EventEmitter<void>();
}
```

### Global Styles

The `styles.scss` file should only contain:

1. CSS reset/normalize to clear browser default styles
2. Root element styles (html, body)
3. Global variables
4. Scrollbar styling
5. Typography defaults

Example of appropriate global styles:

```scss
// styles.scss

// 1. CSS Reset
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

// 2. Root element styles
html,
body {
  @apply h-full bg-gray-50 text-gray-900;
  font-family: 'Inter', sans-serif;
}

// 3. Global variables are handled by Tailwind config

// 4. Scrollbar styling
::-webkit-scrollbar {
  width: 5px;
  height: 5px;
}

::-webkit-scrollbar-thumb {
  @apply bg-gray-400 dark:bg-gray-600;
  border-radius: 50px;
}

::-webkit-scrollbar-track {
  @apply bg-gray-100 dark:bg-gray-800;
}

// 5. Typography defaults
h1 {
  @apply text-2xl font-bold mb-4;
}

h2 {
  @apply text-xl font-semibold mb-3;
}

p {
  @apply mb-4;
}
```

### Color Palette

The application uses a consistent color palette defined in `tailwind.config.js`:

- Primary: `#44bba4` (emerald/teal)
- Background: Light mode `#ffffff`, Dark mode `#1a1a1a`
- Text: Light mode `#000000`, Dark mode `#ffffff`

### Dark Mode

The application supports dark mode using Tailwind's `dark:` variant. Dark mode is class-based and can be toggled with the `dark` class on the `html` element.

```html
<div class="bg-white text-black dark:bg-gray-800 dark:text-white">Content</div>
```

## TypeScript Conventions

### Naming Conventions

- **Files**: Use kebab-case for file names (e.g., `user-profile.component.ts`)
- **Classes**: Use PascalCase for class names (e.g., `UserProfileComponent`)
- **Methods and Properties**: Use camelCase for methods and properties (e.g., `getUserData()`)
- **Interfaces**: Use PascalCase with an `I` prefix (e.g., `IUserProfile`)
- **Enums**: Use PascalCase (e.g., `UserRole`)
- **Constants**: Use UPPER_SNAKE_CASE for global constants (e.g., `MAX_USERS`)

### Code Formatting

The project uses ESLint and Prettier for code formatting. Configuration is defined in:

- `.eslintrc.js`
- `.prettierrc`

Run formatting with:

```bash
npm run format
```

Check formatting with:

```bash
npm run format:check
```

### Import Order

Organize imports in the following order:

1. Angular core imports
2. Third-party library imports
3. Application imports (with a blank line separating from the above)

```typescript
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { UserService } from '@core/services';
import { IUser } from '@shared/models';
```

## Angular Best Practices

### Component Structure

Components should follow this structure:

1. Imports
2. Component decorator
3. Class definition with properties grouped by:
   - Input/Output properties
   - Public properties
   - Private properties
   - Lifecycle methods
   - Public methods
   - Private methods

### Template Guidelines

- Use Angular's modern control flow syntax for better performance and readability
- Limit logic in templates; move complex logic to the component class
- Use the async pipe for handling observables

```html
@for (user of users$ | async; track user.id) {
<div>{{ user.name }}</div>
} @if (isLoading) {
<div>Loading...</div>
} @else {
<div>Content loaded</div>
} @switch (status) { @case ('active') {
<span class="badge-success">Active</span>
} @case ('inactive') {
<span class="badge-warning">Inactive</span>
} @default {
<span class="badge-secondary">Unknown</span>
} }

<!-- Using @defer for lazy loading content -->
@defer {
<heavy-component />
} @loading {
<p>Loading...</p>
}
```

### Reactive Forms

Use reactive forms with form classes instead of FormBuilder for better type safety and testability.

```typescript
// Using form classes instead of FormBuilder
this.userForm = new FormGroup({
  name: new FormControl('', [Validators.required]),
  email: new FormControl('', [Validators.required, Validators.email]),
});

// With typed forms
interface UserForm {
  name: FormControl<string>;
  email: FormControl<string>;
}

this.userForm = new FormGroup<UserForm>({
  name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  email: new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.email],
  }),
});
```

## Visual Design

For comprehensive visual design guidelines, including layout principles, color system, typography, spacing, elevation, iconography, responsive design, and accessibility, please refer to the [Visual Design Guidelines](./visual-design.md) document.
