import { Component, EventEmitter, HostListener, Input, input, Output, output } from '@angular/core';

@Component({
    selector: 'app-infinite-scroll',
    imports: [],
    templateUrl: './infinite-scroll.component.html'
})
export class InfiniteScrollComponent {
    @Input() disabled = false;
    @Input() endOfScroll = false;
    @Output() scrolled = new EventEmitter<void>();

    private scrollThreshold = 0.9;
    private isScrollLocked = false;

    @HostListener('window:scroll')
    onScroll(): void {
        if (this.disabled || this.isScrollLocked) return;

        const scrollPosition = window.scrollY + window.innerHeight;
        const scrollThreshold = document.documentElement.scrollHeight * this.scrollThreshold;

        if (scrollPosition >= scrollThreshold) {
            this.isScrollLocked = true;
            this.scrolled.emit();

            // Unlock scroll when loading is complete
            setTimeout(() => {
                this.isScrollLocked = false;
            }, 1000); // Adjust timeout as needed
        }
    }
} 