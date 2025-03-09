import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface Tab {
    id: string;
    label: string;
}

@Component({
    selector: 'app-tabs',
    imports: [CommonModule],
    templateUrl: './tabs.component.html'
})
export class TabsComponent {
    @Input() tabs: Tab[] = [];
    @Input() activeTab: string = '';
    @Output() tabChange = new EventEmitter<string>();

    setActiveTab(tabId: string): void {
        this.tabChange.emit(tabId);
    }
} 