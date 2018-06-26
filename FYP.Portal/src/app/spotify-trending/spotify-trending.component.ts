import { Component, OnInit } from '@angular/core';
import { MusicService } from '../shared/music.service';
import { DetailedArtist } from '../models/detailed-artist';
import { MainService } from '../shared/main.service';

@Component({
  selector: 'app-spotify-trending',
  templateUrl: './spotify-trending.component.html',
  styleUrls: ['./spotify-trending.component.scss']
})
export class SpotifyTrendingComponent implements OnInit {

  artists: DetailedArtist[];
  selectedArtist: DetailedArtist;
  loading = true;
  collapsed = true;
  spotifyUrl: string;

  constructor(private musicService: MusicService, private helperService: MainService) { }

  ngOnInit() {
    this.musicService.getPopularArtists(7, 5).subscribe(response => {
      response.forEach(x => {
        if (x.spotifyGivenGenre) {
          (<any>x).genreList = x.spotifyGivenGenre.split(',');
        } else {
          (<any>x).genreList = x.otherGenresGivenInRelatedArtists.split(',').slice(0, 2);
        }
      });

      this.artists = response;
      this.selectedArtist = this.artists[0];
      this.selectedArtist.description = decodeURIComponent(this.selectedArtist.description);
      this.setSpotifyUrl(this.selectedArtist);
      this.loading = false;
    });
  }

  getGenreColor(genre: string): string {
    return this.helperService.getColorForGenre(genre);
  }

  selectArtist(index: number): void {
    this.selectedArtist = this.artists[index];
    this.setSpotifyUrl(this.selectedArtist);
  }

  setSpotifyUrl(artist: DetailedArtist): void {
    if (artist.spotifyRecordId) {
      this.spotifyUrl = `https://open.spotify.com/embed/artist/${artist.spotifyRecordId}`;
    } else {
      this.spotifyUrl = null;
    }
  }

}
