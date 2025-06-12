import { Injectable, signal, computed } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly darkModeKey = 'magjob-theme';
  private _isDarkMode = signal(false);

  // Public signal for reactive updates
  isDarkMode = this._isDarkMode.asReadonly();

  constructor() {
    this.initializeTheme();
  }

  private initializeTheme(): void {
    // Check localStorage first, then system preference
    const savedTheme = localStorage.getItem(this.darkModeKey);
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

    const shouldBeDark = savedTheme ? savedTheme === 'dark' : prefersDark;

    this._isDarkMode.set(shouldBeDark);
    this.applyTheme(shouldBeDark);

    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
      if (!localStorage.getItem(this.darkModeKey)) {
        this._isDarkMode.set(e.matches);
        this.applyTheme(e.matches);
      }
    });
  }

  getTheme(): 'dark' | 'light' {
    return this._isDarkMode() ? 'dark' : 'light';
  }

  toggleTheme(): void {
    const newTheme = !this._isDarkMode();
    this._isDarkMode.set(newTheme);
    this.applyTheme(newTheme);
    this.saveTheme(newTheme ? 'dark' : 'light');
  }

  setTheme(theme: 'dark' | 'light'): void {
    const isDark = theme === 'dark';
    this._isDarkMode.set(isDark);
    this.applyTheme(isDark);
    this.saveTheme(theme);
  }

  private applyTheme(isDark: boolean): void {
    const htmlElement = document.documentElement;

    if (isDark) {
      htmlElement.classList.add('dark');
      htmlElement.setAttribute('data-theme', 'dark');
    } else {
      htmlElement.classList.remove('dark');
      htmlElement.setAttribute('data-theme', 'light');
    }
  }

  private saveTheme(theme: 'dark' | 'light'): void {
    localStorage.setItem(this.darkModeKey, theme);
  }
}
