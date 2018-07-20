import { Pipe, PipeTransform } from '@angular/core';
import { Task } from '../models/task';
import { isNgTemplate } from '@angular/compiler';

@Pipe({
  name: 'listsfilter',
  pure: false // Without this, the UI wont update when a new item is added to an array
})
export class ListsfilterPipe implements PipeTransform {

  transform(value: Array<Task>, complete: Boolean): any {
    if(!value) {
      return value;
    }
    if (complete){
      return value.filter(item => item.CompletedDate);
    }else {
      return value.filter(item => !item.CompletedDate);
    }
    
  }

}
