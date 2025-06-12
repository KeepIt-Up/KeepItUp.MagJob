import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

// PrimeNG imports
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-landing',
  imports: [CommonModule, ButtonModule, CardModule],
  templateUrl: './landing.component.html',
})
export class LandingComponent {
  // Component is now purely template-based with no complex logic
}
