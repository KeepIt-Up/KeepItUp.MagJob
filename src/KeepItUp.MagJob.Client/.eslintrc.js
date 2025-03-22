module.exports = {
    root: true,
    ignorePatterns: [
        "dist/**/*",
        "tmp/**/*",
        "out-tsc/**/*",
        "bazel-out/**/*",
        "node_modules/**/*",
        ".idea/**/*",
        ".vscode/**/*",
        ".angular/cache/**/*",
        "coverage/**/*"
    ],
    overrides: [
        {
            files: ['*.ts'],
            parserOptions: {
                project: ['tsconfig.json'],
                createDefaultProgram: true,
                tsconfigRootDir: __dirname
            },
            extends: [
                'plugin:@angular-eslint/recommended',
                'plugin:@angular-eslint/template/process-inline-templates',
                'plugin:prettier/recommended'
            ],
            rules: {
                '@angular-eslint/directive-selector': [
                    'error',
                    {
                        type: 'attribute',
                        prefix: 'app',
                        style: 'camelCase'
                    }
                ],
                '@angular-eslint/component-selector': [
                    'error',
                    {
                        type: 'element',
                        prefix: 'app',
                        style: 'kebab-case'
                    }
                ],
                'prettier/prettier': 'error',
                // Temporarily relax some rules
                '@typescript-eslint/no-explicit-any': 'warn',
                '@typescript-eslint/no-unsafe-return': 'warn',
                '@typescript-eslint/no-unsafe-member-access': 'warn',
                '@typescript-eslint/no-unsafe-assignment': 'warn',
                '@typescript-eslint/no-floating-promises': 'warn',
                '@typescript-eslint/no-unused-vars': 'warn',
                '@typescript-eslint/no-inferrable-types': 'warn'
            }
        },
        {
            files: ['*.html'],
            extends: [
                'plugin:@angular-eslint/template/recommended',
                'plugin:@angular-eslint/template/accessibility',
                'plugin:prettier/recommended'
            ],
            rules: {
                'prettier/prettier': 'error'
            }
        }
    ]
};
