import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SearchInputComponent } from './search-input.component';

describe('SearchInputComponent', () => {
    let component: SearchInputComponent;
    let fixture: ComponentFixture<SearchInputComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [SearchInputComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(SearchInputComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should initialize with default input values', () => {
        expect(component.placeholder()).toBe('');
        expect(component.disabled()).toBe(false);
    });

    it('should emit empty string when clear is called', () => {
        const emitSpy = spyOn(component.valueChange, 'emit');

        component.clear();

        expect(emitSpy).toHaveBeenCalledWith('');
    });
}); 