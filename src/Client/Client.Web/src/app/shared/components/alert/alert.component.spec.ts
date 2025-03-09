import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AlertComponent } from './alert.component';
import { ComponentRef } from '@angular/core';

describe('AlertComponent', () => {
    let component: AlertComponent;
    let componentRef: ComponentRef<AlertComponent>;
    let fixture: ComponentFixture<AlertComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AlertComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(AlertComponent);
        component = fixture.componentInstance;
        componentRef = fixture.componentRef;
        componentRef.setInput('title', 'Test Alert');
        componentRef.setInput('description', 'Test Description');
        componentRef.setInput('showActionButtons', false);
        componentRef.setInput('alertType', 'dark');
        componentRef.setInput('alertVisible', true);
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should show/hide action buttons based on showActionButtons input', () => {
        // Initially hidden
        componentRef.setInput('showActionButtons', false);
        fixture.detectChanges();
        let buttons = fixture.nativeElement.querySelectorAll('button');
        expect(buttons.length).toBe(0);

        // Show buttons
        componentRef.setInput('showActionButtons', true);
        fixture.detectChanges();
        buttons = fixture.nativeElement.querySelectorAll('button');
        expect(buttons.length).toBe(2);
    });

    it('should emit dismissClick event when dismiss button is clicked', () => {
        const dismissSpy = jasmine.createSpy('dismissSpy');
        component.dismissClick.subscribe(dismissSpy);
        componentRef.setInput('showActionButtons', true);
        fixture.detectChanges();

        const dismissButton = fixture.nativeElement.querySelector('button:last-child');
        dismissButton.click();

        expect(dismissSpy).toHaveBeenCalled();
    });

    it('should emit viewMoreClick event when view more button is clicked', () => {
        const viewMoreSpy = jasmine.createSpy('viewMoreSpy');
        component.viewMoreClick.subscribe(viewMoreSpy);
        componentRef.setInput('showActionButtons', true);
        fixture.detectChanges();

        const viewMoreButton = fixture.nativeElement.querySelector('button:first-child');
        viewMoreButton.click();

        expect(viewMoreSpy).toHaveBeenCalled();
    });

    it('should apply correct CSS classes based on alertType', () => {
        const alertTypes: Array<'info' | 'danger' | 'success' | 'warning' | 'dark'> =
            ['info', 'danger', 'success', 'warning', 'dark'];

        alertTypes.forEach(type => {
            componentRef.setInput('alertType', type);
            fixture.detectChanges();

            const alertElement = fixture.nativeElement.querySelector(`[role="alert"]`);
            expect(alertElement.classList.contains(`alert-${type}`)).toBeTrue();
        });
    });

    it('should show/hide alert based on alertVisible input', () => {
        // Initially visible
        componentRef.setInput('alertVisible', true);
        fixture.detectChanges();
        let alertElement = fixture.nativeElement.querySelector(`[role="alert"]`);
        expect(alertElement).toBeTruthy();

        // Hide alert
        componentRef.setInput('alertVisible', false);
        fixture.detectChanges();
        alertElement = fixture.nativeElement.querySelector(`[role="alert"]`);
        expect(alertElement).toBeFalsy();
    });
}); 