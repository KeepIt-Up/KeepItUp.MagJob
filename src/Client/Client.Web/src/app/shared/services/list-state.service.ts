import { Injectable } from '@angular/core';
import { StateService } from './state.service';

@Injectable({
  providedIn: 'root'
})
export class ListStateService<T extends { id: string }, M = undefined> extends StateService<T[], M> {

  add(item: T | T[]) {
    //add only not existing items
    const items = Array.isArray(item) ? item : [item];
    const existingItems = this.state().data ?? [];
    const newItems = items.filter((i) => !existingItems.some((e) => e.id === i.id));
    //update existing items
    const updatedItems = existingItems.map((e) => items.find((i) => i.id === e.id) ?? e);
    this.setData([...updatedItems, ...newItems]);
  }

  remove(item: T) {
    this.setData(this.state().data?.filter((i) => i.id !== item.id) ?? []);
  }

  update(item: T) {
    this.setData(this.state().data?.map((i) => i.id === item.id ? item : i) ?? []);
  }

  get(id: string) {
    return this.state().data?.find((i) => i.id === id);
  }

  setMetadata(metadata: M) {
    this.updateState({ metadata });
  }
}
