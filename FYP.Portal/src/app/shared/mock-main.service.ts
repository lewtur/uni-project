import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';

@Injectable()
export class MockMainService {

  preferencesChanged: Subject<any> = new Subject<any>();
  tweetDataChanged: Subject<any> = new Subject<any>();
  userLoggedInWithSpotify: Subject<any> = new Subject<any>();
  userClickedRecommendedArtist: Subject<any> = new Subject<any>();

  constructor() { }

  updatePreferencesFromCookie(): void { }

  updateTweetData(tweetData: any): void { }

  getColorForGenre(genre: string): string {
    return 'green';
  }

  updateSpotifyToken(token: string): void { }

}
