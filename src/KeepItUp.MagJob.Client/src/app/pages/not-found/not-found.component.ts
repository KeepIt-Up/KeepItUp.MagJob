import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { NgIcon } from '@ng-icons/core';

@Component({
  selector: 'app-not-found',
  imports: [RouterLink, NavbarComponent, ButtonComponent, NgIcon, FooterComponent],
  templateUrl: './not-found.component.html',
})
export class NotFoundComponent {}
