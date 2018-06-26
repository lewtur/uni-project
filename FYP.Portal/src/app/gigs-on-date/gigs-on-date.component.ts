import { Component, OnInit, Input } from '@angular/core';
import { MusicService } from '../shared/music.service';
import { Gig, DisplayGig } from '../models/gig';
import { UserPreferences } from '../models/user-preferences';
import { CookieService } from 'ngx-cookie';
import { Constants } from '../shared/constants';
import { moment } from '../shared/moment';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { MainService } from '../shared/main.service';

@Component({
  selector: 'app-gigs-on-date',
  templateUrl: './gigs-on-date.component.html',
  styleUrls: ['./gigs-on-date.component.scss']
})
export class GigsOnDateComponent implements OnInit {

  @Input()
  dateToDisplay: Date;

  displayGig: DisplayGig;
  relevantGigs: Gig[];
  allGigs: Gig[];
  userPrefs: UserPreferences;
  gigIndex = 0;
  maxGigIndex: number;
  loading = true;

  constructor(
    private musicService: MusicService,
    private cookieService: CookieService,
    private helperService: MainService) {
  }

  ngOnInit() {
    this.userPrefs = <UserPreferences>this.cookieService.getObject(Constants.UserPrefsCookieKey);

    this.musicService.getGigs(this.dateToDisplay).subscribe(result => {
      this.allGigs = result;
      this.filterAndOrderGigs();
      this.setDisplayGig();
      this.loading = false;
    });

    this.helperService.preferencesChanged.subscribe(prefs => {
      this.loading = true;
      this.userPrefs = <UserPreferences>prefs;
      this.filterAndOrderGigs();
      this.setDisplayGig();
      this.loading = false;
    });
  }

  filterAndOrderGigs(): void {
    if (this.userPrefs && this.userPrefs.cities) {
      this.relevantGigs = this.allGigs.filter(x => this.userPrefs.cities.includes(x.venue.town));
    } else {
      this.relevantGigs = this.allGigs;
    }

    this.relevantGigs = this.relevantGigs.sort((a, b) => moment(a.event.startDate).diff(moment(b.event.startDate)));

    if (!this.relevantGigs || !this.relevantGigs.length) {
      this.relevantGigs = this.allGigs;
    }

    this.maxGigIndex = this.relevantGigs.length;
  }

  setDisplayGig(): void {
    const newGig = new DisplayGig();
    const gig = this.relevantGigs[this.gigIndex];

    newGig.eventName = gig.event.name;
    newGig.venueLatitude = +gig.venue.latitude;
    newGig.venueLongitude = +gig.venue.longitude;
    newGig.venueName = gig.venue.name;
    newGig.startTime = gig.event.doorsClose;
    newGig.description = gig.event.description;
    newGig.eventDate = gig.event.startDate;
    newGig.town = gig.venue.town;

    if (gig.artist.length && gig.artist[0]) {
      if (gig.artist[0].spotifyRecordId) {
        newGig.artistSpotifyUrl = `https://open.spotify.com/embed/artist/${gig.artist[0].spotifyRecordId}`;
      } else if (gig.artist[0].name) {
        newGig.artistGoogleLink = `https://www.google.co.uk/search?q=${gig.artist[0].name}`;
      }

      if (gig.artist[0].spotifyGivenGenre) {
        newGig.artistGenres = gig.artist[0].spotifyGivenGenre.split(',');
      } else if (gig.artist[0].otherGenresGivenInRelatedArtists) {
        newGig.artistGenres = gig.artist[0].otherGenresGivenInRelatedArtists.split(',').slice(0, 2);
      }
    } else {
      newGig.artistGoogleLink = `https://www.google.co.uk/search?q=${gig.event.name}`;
    }

    this.displayGig = newGig;
  }

  getGenreColor(genre: string): string {
    return this.helperService.getColorForGenre(genre);
  }

  moveDayForwards(): void {
    this.gigIndex++;
    this.setDisplayGig();
  }

  moveDayBackwards(): void {
    this.gigIndex--;
    this.setDisplayGig();
  }
}
