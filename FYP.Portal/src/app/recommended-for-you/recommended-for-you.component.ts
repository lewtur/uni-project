import { Component, OnInit } from '@angular/core';
import { MainService } from '../shared/main.service';
import { MusicService } from '../shared/music.service';
import { UserRecommendedArtist, SinglePopularityFeature } from '../models/artist-with-features';
import { KeyRegistry } from '@angular/core/src/di/reflective_key';
import { Constants } from '../shared/constants';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-recommended-for-you',
  templateUrl: './recommended-for-you.component.html',
  styleUrls: ['./recommended-for-you.component.scss']
})
export class RecommendedForYouComponent implements OnInit {

  private albumKey = 'album-';
  private cityKey = 'city-';
  private labelKey = 'label-';
  private venueKey = 'venue-';
  private gigKey = 'gig-';

  artists: UserRecommendedArtist[];
  selectedArtist: UserRecommendedArtist;
  showMoreMessage = false;
  spotifyUrl: string;

  showSpotifyPrompt = false;

  constructor(
    private helperService: MainService,
    private musicService: MusicService
  ) { }

  ngOnInit() {
    this.helperService.userLoggedInWithSpotify.subscribe(token => {
      this.initPanels(token);
    });
  }

  initPanels(token: string): void {
    this.musicService.getUsersTopTracks(token).subscribe(results => {

      const userGenres = [];

      if (!results || (<any>results).failed) {
        this.showSpotifyPrompt = true;
      } else {
        this.showSpotifyPrompt = false;
        (<any>results).items.forEach(artist => {
          artist.genres.forEach(genre => {
            if (!userGenres.includes(genre)) {
              userGenres.push(genre);
            }
          });
        });
      }

      this.musicService.getUsersSuggestedArtists(userGenres.join(',')).subscribe(x => {
        this.artists = x;
        this.artists.forEach(a => {
          if (!a.genre) {

          } else if (a.genre.spotifyGivenGenre && a.genre.spotifyGivenGenre.length) {
            a.genreList = a.genre.spotifyGivenGenre.split(',').slice(0, 4);
          } else if (a.genre.mostPopularGenreOfRelatedArtists && a.genre.mostPopularGenreOfRelatedArtists.length) {
            a.genreList = a.genre.mostPopularGenreOfRelatedArtists.split(',').slice(0, 4);
          }

          a.features.forEach(f => this.setFeatureText(f));
        });

        if (this.artists && this.artists.length) {
          this.selectArtist(this.artists[0]);
        }
      });
    });
  }

  getGenreColor(genre: string): string {
    return this.helperService.getColorForGenre(genre);
  }

  selectArtist(artist: UserRecommendedArtist): void {
    this.artists.map(x => x.selected = false);
    this.artists = this.artists.filter(x => x.artist.id !== artist.artist.id);
    this.artists.push(artist);

    this.spotifyUrl = `https://open.spotify.com/embed/artist/${artist.artist.spotifyRecordId}`;

    this.helperService.userClickedRecommendedArtist.next(artist.artist.name);
    artist.selected = true;
  }

  setFeatureText(feature: SinglePopularityFeature): void {
    let startingText = '';
    let keyUsed = '';

    if (feature.term.startsWith(this.albumKey)) {
      keyUsed = this.albumKey;
      startingText = 'Released a record on a ';

    } else if (feature.term.startsWith(this.labelKey)) {
      keyUsed = this.labelKey;
      startingText = 'Released a record under ';

    } else if (feature.term.startsWith(this.gigKey)) {
      keyUsed = this.gigKey;
      startingText = 'Played a gig on a ';

    } else if (feature.term.startsWith(this.venueKey)) {
      keyUsed = this.venueKey;
      startingText = 'Played a gig at ';

    } else if (feature.term.startsWith(this.cityKey)) {
      keyUsed = this.cityKey;
      startingText = 'Played a gig in ';

    }

    const description = this.toTitleCase(feature.term.slice(keyUsed.length));

    feature.firstHalf = startingText;
    feature.secondHalf = description;
  }

  toTitleCase(str): string {
    return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
  }

}
