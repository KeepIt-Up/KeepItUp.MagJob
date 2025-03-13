// @ts-check
const eslint = require('@eslint/js');
const tseslint = require('typescript-eslint');
const angular = require('angular-eslint');
const prettierPlugin = require('eslint-plugin-prettier');

module.exports = tseslint.config(
  {
    ignores: [
      // Compiled output
      '**/dist/**',
      '**/tmp/**',
      '**/out-tsc/**',
      '**/bazel-out/**',

      // Node
      '**/node_modules/**',
      '**/*.log',

      // IDEs and editors
      '**/.idea/**',
      '**/.project',
      '**/.classpath',
      '**/.c9/**',
      '**/*.launch',
      '**/.settings/**',
      '**/*.sublime-workspace',

      // Visual Studio Code
      '**/.vscode/**',
      '**/.history/**',

      // Miscellaneous
      '**/.angular/cache/**',
      '**/.sass-cache/**',
      '**/connect.lock',
      '**/coverage/**',
      '**/libpeerconnection.log',
      '**/testem.log',
      '**/typings/**',

      // System files
      '**/.DS_Store',
      '**/Thumbs.db',

      // HTML files
      '**/*.html',
    ],
    files: ['**/*.ts'],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommendedTypeChecked,
      ...tseslint.configs.stylisticTypeChecked,
      ...angular.configs.tsRecommended,
    ],
    languageOptions: {
      parser: tseslint.parser,
      parserOptions: {
        project: './tsconfig.json',
        tsconfigRootDir: __dirname,
        ecmaVersion: 'latest',
        sourceType: 'module',
      },
    },
    processor: angular.processInlineTemplates,
    plugins: {
      prettier: prettierPlugin,
    },
    rules: {
      '@angular-eslint/directive-selector': [
        'error',
        {
          type: 'attribute',
          prefix: 'app',
          style: 'camelCase',
        },
      ],
      '@angular-eslint/component-selector': [
        'error',
        {
          type: 'element',
          prefix: 'app',
          style: 'kebab-case',
        },
      ],
      'prettier/prettier': 'error',
      // Temporarily disable some rules that are causing many errors
      '@typescript-eslint/no-explicit-any': 'warn',
      '@typescript-eslint/no-unsafe-return': 'warn',
      '@typescript-eslint/no-unsafe-member-access': 'warn',
      '@typescript-eslint/no-unsafe-assignment': 'warn',
      '@typescript-eslint/no-floating-promises': 'warn',
      '@typescript-eslint/no-unused-vars': 'warn',
      '@typescript-eslint/no-inferrable-types': 'warn',
    },
  },
  {
    files: ['**/*.html'],
    extends: [...angular.configs.templateRecommended, ...angular.configs.templateAccessibility],
    rules: {
      'prettier/prettier': 'error',
    },
    plugins: {
      prettier: prettierPlugin,
    },
  },
);
