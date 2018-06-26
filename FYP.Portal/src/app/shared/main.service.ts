import { Injectable } from '@angular/core';
import { EventEmitter } from 'selenium-webdriver';
import { UserPreferences } from '../models/user-preferences';
import { Subject } from 'rxjs/Subject';
import { CookieService } from 'ngx-cookie';
import { Constants } from './constants';
import { FullArtistTweets } from '../models/full-artist-tweets';

@Injectable()
export class MainService {

  preferencesChanged: Subject<UserPreferences>;
  tweetDataChanged: Subject<FullArtistTweets>;
  userLoggedInWithSpotify: Subject<string>;
  userClickedRecommendedArtist: Subject<string>;

  colorDict: { [key: string]: string } = {};

  constructor(
    private cookieService: CookieService
  ) {
    this.preferencesChanged = new Subject<UserPreferences>();
    this.tweetDataChanged = new Subject<FullArtistTweets>();
    this.userLoggedInWithSpotify = new Subject<string>();
    this.userClickedRecommendedArtist = new Subject<string>();
  }

  updatePreferencesFromCookie(): void {
    const prefs = <UserPreferences>this.cookieService.getObject(Constants.UserPrefsCookieKey);
    this.preferencesChanged.next(prefs);
  }

  updateTweetData(tweetData: FullArtistTweets): void {
    this.tweetDataChanged.next(tweetData);
  }

  updateSpotifyToken(token: string): void {
    this.userLoggedInWithSpotify.next(token);
  }

  getColorForGenre(genre: string): string {
    if (!this.colorDict[genre]) {
      this.colorDict[genre] = this.getRandomColor();
    }

    return this.colorDict[genre];
  }

  private getRandomColor(): string {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

}
