import { Component, input, output } from "@angular/core";


@Component({
  selector: 'app-search-input',
  templateUrl: './search-input.component.html'
})
export class SearchInputComponent {
  placeholder = input<string>('');
  disabled = input<boolean>(false);
  valueChange = output<string>();

  clear(): void {
    this.valueChange.emit('');
  }
}
