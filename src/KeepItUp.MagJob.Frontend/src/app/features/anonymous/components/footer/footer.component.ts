import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-footer',
  imports: [CommonModule, RouterLink, ButtonModule, DividerModule, TooltipModule],
  templateUrl: './footer.component.html',
})
export class FooterComponent {}
