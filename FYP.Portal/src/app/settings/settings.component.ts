import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MusicService } from '../shared/music.service';
import { City } from '../models/city';
import { UserPreferences } from '../models/user-preferences';
import { CookieService } from 'ngx-cookie';
import { Constants } from '../shared/constants';
import { MainService } from '../shared/main.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  // tslint:disable-next-line:max-line-length
  linkAccountUrl = `https://accounts.spotify.com/authorize?client_id=***REMOVED***&response_type=token&redirect_uri=${environment.spotifyCallback}&scope=user-top-read`;

  openValue = false;
  @Output() openChange = new EventEmitter();

  @Input()
  get open() {
    return this.openValue;
  }

  set open(val) {
    this.openValue = val;
    this.openChange.emit(this.openValue);
  }

  cities: City[] = [];
  prefs: UserPreferences;

  constructor(
    private musicServce: MusicService,
    private cookieService: CookieService,
    private helper: MainService) { }

  ngOnInit() {
    this.musicServce.getCities().subscribe(result => this.cities = result);

    this.prefs = <UserPreferences>this.cookieService.getObject(Constants.UserPrefsCookieKey);
    if (this.prefs) {
      this.cities.map(x => {
        x.selected = this.prefs.cities.includes(x.name);
      });
    }
  }

  selectCity(city: City): void {
    city.selected = !city.selected;

    const userPrefs = new UserPreferences();
    userPrefs.cities = this.cities.filter(x => x.selected).map(y => y.name);

    const expiryDate = new Date();
    expiryDate.setFullYear(expiryDate.getFullYear() + 1);
    this.cookieService.putObject(Constants.UserPrefsCookieKey, userPrefs, {expires: expiryDate});
    this.helper.updatePreferencesFromCookie();
  }

  clickedOutside(event: Event) {
    if (!event || !event.srcElement || !event.srcElement.classList) {
      return;
    }

    if (!event.srcElement.classList.contains('fa-bars')) {
      this.open = false;
    }
  }
}
