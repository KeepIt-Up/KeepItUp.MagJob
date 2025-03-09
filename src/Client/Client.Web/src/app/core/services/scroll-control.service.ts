import { Injectable, signal } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ScrollControlService {
    private scrollable = signal(true);
    scrollable$ = this.scrollable;

    setScrollable(isScrollable: boolean) {
        this.scrollable.set(isScrollable);
    }
} 