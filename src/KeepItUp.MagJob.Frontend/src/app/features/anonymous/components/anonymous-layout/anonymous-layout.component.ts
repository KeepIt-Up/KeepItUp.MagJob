import { Component } from '@angular/core';
import { TopbarComponent } from '@features/anonymous/components/topbar/topbar.component';
import { FooterComponent } from '../footer/footer.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-anonymous-layout',
  imports: [TopbarComponent, RouterOutlet, FooterComponent],
  templateUrl: './anonymous-layout.component.html',
})
export class AnonymousLayoutComponent {}
