# Code Formatting Tools

This project includes several tools to help maintain consistent code formatting and style.

## Available Tools

1. **Prettier** - An opinionated code formatter that enforces a consistent style.
2. **ESLint** - A static code analysis tool for identifying problematic patterns.
3. **ESLint + Prettier Integration** - Combines both tools for optimal code quality.

## Configuration Files

- `.prettierrc` - Contains Prettier configuration settings.
- `.prettierignore` - Lists files and directories to be ignored by Prettier.
- `eslint.config.js` - Contains ESLint configuration with Prettier integration.
- `.eslintrc.js` - Contains Angular-specific ESLint configuration.

## Available Scripts

Run these commands from the project root:

### Format Code

```bash
npm run format
```

This command will format all TypeScript, HTML, SCSS, CSS, and JSON files in the `src` directory according to the rules defined in `.prettierrc`.

### Check Formatting

```bash
npm run format:check
```

This command checks if your code is formatted according to Prettier rules without making any changes.

### Lint Code

```bash
npm run lint
```

This command runs ESLint to check for linting issues.

### Format and Lint

```bash
npm run lint-format
```

This command runs both the linter and formatter in sequence.

## Current Linting Configuration

Some ESLint rules have been temporarily relaxed to allow for a gradual transition to stricter typing and better practices. The following rules are currently set to "warn" instead of "error":

- `@typescript-eslint/no-explicit-any` - Warns about using the `any` type
- `@typescript-eslint/no-unsafe-return` - Warns about returning values of type `any`
- `@typescript-eslint/no-unsafe-member-access` - Warns about accessing properties on values of type `any`
- `@typescript-eslint/no-unsafe-assignment` - Warns about assigning values of type `any`
- `@typescript-eslint/no-floating-promises` - Warns about unhandled promises
- `@typescript-eslint/no-unused-vars` - Warns about unused variables
- `@typescript-eslint/no-inferrable-types` - Warns about explicit type declarations for variables initialized with a literal

These rules should be gradually fixed in the codebase and eventually set back to "error" for stricter enforcement.

## Using with VS Code

For the best experience, install the following VS Code extensions:

1. **ESLint** - Microsoft's ESLint extension
2. **Prettier - Code formatter** - Prettier's official extension

Then, configure VS Code to format on save by adding these settings to your `settings.json`:

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  }
}
```

## Manual Setup for Pre-commit Hooks (Optional)

If you want to set up pre-commit hooks to automatically format code before commits:

1. Initialize Husky:

   ```bash
   npx husky install
   ```

2. Add a pre-commit hook:
   ```bash
   npx husky add .husky/pre-commit "npx lint-staged"
   ```

This will ensure your code is properly formatted before each commit.

## Troubleshooting

If you encounter any issues with the formatting tools:

1. Make sure all dependencies are installed:

   ```bash
   npm install
   ```

2. Try clearing the ESLint cache:

   ```bash
   npx eslint --cache --cache-location ./node_modules/.cache/eslint/ --fix
   ```

3. Check for conflicts between ESLint and Prettier rules.
