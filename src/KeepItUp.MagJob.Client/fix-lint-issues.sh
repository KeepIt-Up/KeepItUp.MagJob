#!/bin/bash

echo "Fixing common linting issues..."

echo "Running ESLint with --fix option..."
npx eslint --fix "src/**/*.ts" "src/**/*.html"

echo "Running Prettier to format code..."
npm run format

echo "Linting and formatting completed!"
echo
echo "Note: Some issues may still require manual fixes."
echo "Check the documentation in CODE_FORMATTING.md for more information."
echo
