import { Pipe, PipeTransform } from '@angular/core';
import { moment } from '../shared/moment';

@Pipe({
  name: 'friendlyDate'
})
export class FriendlyDatePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    const date = moment(value);
    const diff = Math.abs(moment().diff(date, 'days'));

    if (diff === 0) {
      return 'Tonight';
    } else if (diff === 1) {
      return 'Tomorrow';
    } else if (diff < 7) {
      return date.format('dddd');
    } else if (diff < 14) {
      return `Next ${date.format('dddd')}`;
    } else {
      return date.format('do MMM');
    }
  }

}
