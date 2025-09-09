import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkEvidenceComponent } from '../../features/workevidence/workevidence.component';

@Component({
  selector: 'app-workevidence-page',
  standalone: true,
  imports: [CommonModule, WorkEvidenceComponent],
  templateUrl: './workevidence-page.component.html',
  styleUrls: ['./workevidence-page.component.scss']
})
export class WorkEvidencePageComponent {

}
// This component serves as a container for the WorkEvidenceComponent, allowing it to be used as a page in the application.