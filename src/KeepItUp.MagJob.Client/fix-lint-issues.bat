@echo off
echo Fixing common linting issues...

echo Running ESLint with --fix option...
call npx eslint --fix "src/**/*.ts" "src/**/*.html"

echo Running Prettier to format code...
call npm run format

echo Linting and formatting completed!
echo.
echo Note: Some issues may still require manual fixes.
echo Check the documentation in CODE_FORMATTING.md for more information.
echo.

pause
