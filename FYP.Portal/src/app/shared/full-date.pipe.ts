import { Pipe, PipeTransform } from '@angular/core';
import { moment } from '../shared/moment';

@Pipe({
  name: 'fullDate'
})
export class FullDatePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return moment(value).format('dddd Do MMMM, YYYY');
  }

}
