import { Pipe, PipeTransform, Type } from '@angular/core';
import { inject, Injector } from '@angular/core';

@Pipe({
    name: 'dynamicPipe'
})
export class DynamicPipe implements PipeTransform {
    private injector = inject(Injector);

    transform(value: any, pipe: Type<any>, args: any[] = []): any {
        if (!pipe) return value;

        const pipeInstance = this.injector.get(pipe);
        return pipeInstance.transform(value, ...(args || []));
    }
} 