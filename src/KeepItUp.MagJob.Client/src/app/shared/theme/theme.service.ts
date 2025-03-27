import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly darkModeKey = 'color-theme';
  private isDarkMode = false;

  constructor() {
    this.isDarkMode = document.documentElement.classList.contains('dark');

    if (
      localStorage.getItem(this.darkModeKey) === 'dark' ||
      (!localStorage.getItem(this.darkModeKey) &&
        window.matchMedia('(prefers-color-scheme: dark)').matches)
    ) {
      document.documentElement.classList.add('dark');
      this.isDarkMode = true;
    } else {
      document.documentElement.classList.remove('dark');
      this.isDarkMode = false;
    }
  }

  getTheme(): 'dark' | 'light' {
    return localStorage.getItem(this.darkModeKey) === 'dark' ? 'dark' : 'light';
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;

    if (this.isDarkMode) {
      document.documentElement.classList.add('dark');
      this.setTheme('dark');
    } else {
      document.documentElement.classList.remove('dark');
      this.setTheme('light');
    }
  }

  private setTheme(theme: 'dark' | 'light'): void {
    localStorage.setItem(this.darkModeKey, theme);
  }
}
