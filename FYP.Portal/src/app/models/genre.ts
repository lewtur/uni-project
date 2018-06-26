import { IMenuItem } from './menu-item';

export class Genre implements IMenuItem {
    name: string;
    selected = false;

    constructor(name: string) {
      this.name = name;
    }
}
