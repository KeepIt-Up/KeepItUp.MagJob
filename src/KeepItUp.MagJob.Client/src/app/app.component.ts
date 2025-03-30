import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AlertContainerComponent } from '@shared/components/alert-container/alert-container.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AlertContainerComponent],
  template: `
    <router-outlet></router-outlet>
    <app-alert-container></app-alert-container>
  `,
  styles: [],
})
export class AppComponent {}
