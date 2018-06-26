import { Component, OnInit, AfterViewInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { MusicService } from '../shared/music.service';
import { NgwWowService } from 'ngx-wow';
import { City } from '../models/city';
import { trigger, state, style, transition, animate, keyframes } from '@angular/animations';
import { Genre } from '../models/genre';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/forkJoin';
import { IMenuItem } from '../models/menu-item';
import { CookieService } from 'ngx-cookie';
import { UserPreferences } from '../models/user-preferences';
import { SetupAnimations } from './first-time-setup.animations';
import { Constants } from '../shared/constants';
import { MainService } from '../shared/main.service';

@Component({
  selector: 'app-first-time-setup',
  templateUrl: './first-time-setup.component.html',
  styleUrls: ['./first-time-setup.component.scss'],
  animations: SetupAnimations
})
export class FirstTimeSetupComponent implements OnInit {

  cities: City[];
  genres: Genre[];
  cityIsSelected = false;
  finished = false;
  chooseMessage = 'Choose your city';

  constructor(
    public activeModal: NgbActiveModal,
    private musicService: MusicService,
    private wowService: NgwWowService,
    private cookieService: CookieService,
    private helper: MainService) {
  }

  ngOnInit() {
    const citiesCall = this.musicService.getCities().subscribe(res => {
      this.cities = res;
      this.wowService.init();
    });
  }

  selectMenutem(item: IMenuItem): void {
    item.selected = !item.selected;
    this.cityIsSelected = this.cities.some(x => x.selected);
  }

  clickNext(): void {
    const userPrefs = new UserPreferences();
    userPrefs.cities = this.cities.filter(x => x.selected).map(y => y.name);

    const expiryDate = new Date();
    expiryDate.setFullYear(expiryDate.getFullYear() + 1);

    this.cookieService.putObject(Constants.UserPrefsCookieKey, userPrefs, {expires: expiryDate});
    this.helper.updatePreferencesFromCookie();
    this.finished = true;
  }
}
