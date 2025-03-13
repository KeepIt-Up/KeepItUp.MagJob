import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from '@core/components/navbar/navbar.component';

@Component({
    selector: 'app-public',
    imports: [RouterModule, NavbarComponent],
    templateUrl: './public.component.html'
})
export class PublicComponent {

}
