import { Component, inject, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit, effect } from '@angular/core';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { NgIcon } from '@ng-icons/core';
import { AuthService } from '@core/services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Shift } from './models/shift.model';
import { ShiftApiService } from './services/shift.api';
import { ShiftEditRequestService } from '../workevidence/services/shiftEditRequest.service';
import { ShiftEditRequest } from './models/shiftEditRequest.model';
import { PaginatedResponse } from '@shared/components/pagination/pagination.component';
import Chart from 'chart.js/auto';
import { Subscription } from 'rxjs';
import { UserContextService } from '@users/services/user-context.service';
import { UserService } from '@users/services/user.service';

@Component({
  selector: 'app-shift',
  templateUrl: './shift.component.html',
  styleUrls: ['./shift.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NavbarComponent,
    ButtonComponent,
    FooterComponent
  ]
})
export class ShiftComponent implements OnInit, OnDestroy {
  private shiftApiService = inject(ShiftApiService);
  private shiftEditRequestService = inject(ShiftEditRequestService);
  private userService = inject(UserService);
  private subscription = new Subscription();
  
  currentShift: Shift | null = null;
  isLoading = false;
  error: string | null = null;
  description: string = '';
  
  editRequests: ShiftEditRequest[] = [];
  showEditRequestForm = false;
  newStartTime: string | null = null;
  newEndTime: string | null = null;
  editRequestDescription: string = '';
  isSubmitting = false;
  isEndingShift = false;
  errorMessage = '';

  constructor() {
    effect(() => {
      const state = this.shiftEditRequestService.shiftEditRequestsState$();
      if (state.data) {
        this.editRequests = state.data.items;
      }
      if (state.error) {
        this.error = 'Nie udało się załadować wniosków o zmianę.';
        console.error('Error loading edit requests:', state.error);
      }
    });
  }

  ngOnInit() {
    this.checkActiveShift();
    this.editRequestDescription = '';
    this.newStartTime = null;
    this.newEndTime = null;
    this.showEditRequestForm = false;
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  checkActiveShift() {
    this.isLoading = true;
    this.error = null;
    
    this.shiftApiService.getActiveShift().subscribe({
      next: (shift) => {
        console.log('Active shift:', shift);
        this.currentShift = shift;
        this.editRequests = shift.shiftEditRequests || [];
        this.isLoading = false;
      },
      error: (err) => {
        if (err.status === 404) {
          this.currentShift = null;
          this.editRequests = [];
          this.error = null;
        } else {
          this.error = 'Nie udało się sprawdzić aktywnej zmiany.';
          console.error('Error checking active shift:', err);
        }
        this.isLoading = false;
      }
    });
    console.log(this.currentShift);
  }

  startShift() {
    if (!this.description) {
      this.error = 'Proszę wprowadzić opis zmiany';
      return;
    }

    const memberId = this.userService.userContext!.id;

    this.isLoading = true;
    this.error = null;

    const payload = {
      startTime: new Date().toISOString(),
      description: this.description,
      memberId: memberId
    };

    this.shiftApiService.startShift(payload).subscribe({
      next: (shift) => {
        this.currentShift = shift;
        this.editRequests = shift.shiftEditRequests || [];
        this.description = shift.description || '';
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Nie udało się rozpocząć zmiany. Spróbuj ponownie.';
        this.isLoading = false;
      }
    });
  }

  endShift() {
    if (!this.currentShift) return;

    this.isLoading = true;
    this.error = null;

    this.shiftApiService.endShift(this.currentShift.id).subscribe({
      next: () => {
        this.currentShift = null;
        this.editRequests = [];
      },
      error: (err) => {
        this.error = 'Nie udało się zakończyć zmiany. Spróbuj ponownie.';
        console.error('Error ending shift:', err);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  createEditRequest() {
    if (!this.currentShift || !this.newStartTime || !this.newEndTime) {
      this.error = 'Proszę wypełnić wszystkie wymagane pola';
      return;
    }

    if (!this.editRequestDescription) {
      this.error = 'Proszę wprowadzić opis zmiany';
      return;
    }

    const payload = {
      newStartTime: this.newStartTime,
      newEndTime: this.newEndTime,
      status: 'PENDING',
      shiftId: this.currentShift.id,
      description: this.editRequestDescription
    };

    this.shiftEditRequestService.createShiftEditRequest(payload).subscribe({
      next: () => {
        this.showEditRequestForm = false;
        this.newStartTime = null;
        this.newEndTime = null;
        this.editRequestDescription = '';
        this.error = null;
        this.shiftApiService.getShiftById(this.currentShift!.id).subscribe({
          next: (shift: Shift) => {
            this.currentShift = shift;
            this.editRequests = shift.shiftEditRequests || [];
            console.log('Edit requests:', this.editRequests);
          },
          error: (err: Error) => {
            this.error = 'Nie udało się odświeżyć danych zmiany.';
            console.error('Error refreshing shift data:', err);
          }
        });
      },
      error: (err: Error) => {
        this.error = 'Nie udało się utworzyć wniosku o zmianę.';
        console.error('Error creating edit request:', err);
      }
    });
  }

  deleteEditRequest(id: string) {
    this.shiftEditRequestService.deleteShiftEditRequest(id).subscribe({
      next: () => {

        if (this.currentShift) {
          this.shiftApiService.getShiftById(this.currentShift.id).subscribe({
            next: (shift: Shift) => {
              this.currentShift = shift;
              this.editRequests = shift.shiftEditRequests || [];
            },
            error: (err: Error) => {
              this.error = 'Nie udało się odświeżyć danych zmiany.';
              console.error('Error refreshing shift data:', err);
            }
          });
        }
      },
      error: (err: Error) => {
        this.error = 'Nie udało się usunąć wniosku o zmianę.';
        console.error('Error deleting edit request:', err);
      }
    });
  }

  acceptEditRequest(requestId: string) {
    this.isLoading = true;
    this.error = null;

    const request = this.editRequests.find(r => r.id === requestId);
    if (!request) {
      this.error = 'Nie znaleziono wniosku.';
      this.isLoading = false;
      return;
    }

    const payload = {
      startTime: request.startTime,
      endTime: request.endTime,
      status: 'accepted',
      description: request.description || ''
    };


    this.shiftEditRequestService.updateShiftEditRequest(requestId, payload).subscribe({
      next: () => {
        if (this.currentShift) {
          this.shiftApiService.getShiftById(this.currentShift.id).subscribe({
            next: (shift: Shift) => {
              this.currentShift = shift;
              this.editRequests = shift.shiftEditRequests || [];
              this.isLoading = false;
            },
            error: (err: Error) => {
              this.error = 'Nie udało się odświeżyć danych zmiany.';
              console.error('Error refreshing shift data:', err);
              this.isLoading = false;
            }
          });
        }
      },
      error: (err: Error) => {
        this.error = 'Nie udało się zaakceptować wniosku.';
        console.error('Error accepting edit request:', err);
        this.isLoading = false;
      }
    });
  }
}
