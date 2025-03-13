import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaginationComponent } from './pagination.component';
import { CommonModule } from '@angular/common';

describe('PaginatedListComponent', () => {
    let component: PaginationComponent<any>;
    let fixture: ComponentFixture<PaginationComponent<any>>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CommonModule, PaginationComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(PaginationComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('totalPages', () => {
        it('should calculate total pages correctly', () => {
            component.totalItems = 25;
            component.pageSize = 10;
            expect(component.totalPages).toBe(3);
        });

        it('should handle zero items', () => {
            component.totalItems = 0;
            component.pageSize = 10;
            expect(component.totalPages).toBe(0);
        });
    });

    describe('paginatedItems', () => {
        beforeEach(() => {
            component.items = Array.from({ length: 25 }, (_, i) => i + 1); // [1,2,3,...,25]
            component.pageSize = 10;
        });

        it('should return correct items for first page', () => {
            component.currentPage = 1;
            expect(component.paginatedItems.length).toBe(10);
            expect(component.paginatedItems[0]).toBe(1);
            expect(component.paginatedItems[9]).toBe(10);
        });

        it('should return correct items for middle page', () => {
            component.currentPage = 2;
            expect(component.paginatedItems.length).toBe(10);
            expect(component.paginatedItems[0]).toBe(11);
            expect(component.paginatedItems[9]).toBe(20);
        });

        it('should return remaining items for last page', () => {
            component.currentPage = 3;
            expect(component.paginatedItems.length).toBe(5);
            expect(component.paginatedItems[0]).toBe(21);
            expect(component.paginatedItems[4]).toBe(25);
        });
    });

    describe('onPageChange', () => {
        let pageChangeEmitted: number;

        beforeEach(() => {
            component.totalItems = 30;
            component.pageSize = 10;
            component.currentPage = 1;
            component.pageChange.subscribe((page: number) => {
                pageChangeEmitted = page;
            });
        });

        it('should emit page change when valid page selected', () => {
            component.onPageChange(2);
            expect(component.currentPage).toBe(2);
            expect(pageChangeEmitted).toBe(2);
        });

        it('should not emit page change for invalid page numbers', () => {
            component.currentPage = 2;
            pageChangeEmitted = 2;

            component.onPageChange(0);
            expect(component.currentPage).toBe(2);
            expect(pageChangeEmitted).toBe(2);

            component.onPageChange(4);
            expect(component.currentPage).toBe(2);
            expect(pageChangeEmitted).toBe(2);
        });

        it('should update pagesNumbers correctly', () => {
            component.onPageChange(2);
            expect(component.pagesNumbers).toEqual([1, 2, 3]);
        });
    });

    describe('onPageSizeChange', () => {
        let pageChangeEmitted: number;
        let pageSizeChangeEmitted: number;

        beforeEach(() => {
            component.totalItems = 30;
            component.pageSize = 10;
            component.currentPage = 2;

            component.pageChange.subscribe((page: number) => {
                pageChangeEmitted = page;
            });
            component.pageSizeChange.subscribe((size: number) => {
                pageSizeChangeEmitted = size;
            });
        });

        it('should handle numeric input and emit both page size and page change', () => {
            component.onPageSizeChange(20);

            expect(component.pageSize).toBe(20);
            expect(component.currentPage).toBe(1);
            expect(pageSizeChangeEmitted).toBe(20);
            expect(pageChangeEmitted).toBe(1);
        });

        it('should handle string input and emit both page size and page change', () => {
            component.onPageSizeChange('15');

            expect(component.pageSize).toBe(15);
            expect(component.currentPage).toBe(1);
            expect(pageSizeChangeEmitted).toBe(15);
            expect(pageChangeEmitted).toBe(1);
        });
    });
}); 