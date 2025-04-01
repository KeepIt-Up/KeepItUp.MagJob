import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { InputComponent } from '../../shared/components/input/input.component';
import { NgIcon } from '@ng-icons/core';
import { FooterComponent } from '@shared/components';

@Component({
  selector: 'app-help',
  imports: [NavbarComponent, ButtonComponent, InputComponent, FormsModule, NgIcon, FooterComponent],
  templateUrl: './help.component.html',
  styleUrl: './help.component.scss',
})
export class HelpComponent {
  searchQuery = '';

  onSearch(): void {
    console.log('Searching for:', this.searchQuery);
    // Implement search functionality
  }
}
