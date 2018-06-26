import { Injectable } from '@angular/core';
import { DetailedArtist } from '../models/detailed-artist';
import { Observable } from 'rxjs/Observable';
import { Gig, Event, Venue } from '../models/gig';
import { Album } from '../models/album';
import { Artist } from '../models/artist';
import { FullArtistStats, Genre } from '../models/full-artist-stats';
import { FullArtistTweets } from '../models/full-artist-tweets';
import { City } from '../models/city';

@Injectable()
export class MockMusicService {

  constructor() { }

  getPopularArtists(daysToLookBack: number, limit: number): Observable<DetailedArtist[]> {
    const artist = new DetailedArtist();
    artist.spotifyGivenGenre = 'potato,waffle';
    return Observable.of([artist]);
  }

  getGigs(date: Date): Observable<Gig[]> {
    const gig = new Gig();
    gig.artist = [new DetailedArtist()];
    gig.event = new Event();
    gig.venue = new Venue();
    return Observable.of([gig]);
  }

  getAlbums(date: Date): Observable<Album[]> {
    const album = new Album();
    album.genres = 'egg,and,beans';
    return Observable.of([album]);
  }

  getArtists(query: string): Observable<Artist[]> {
    return Observable.of([new Artist()]);
  }

  getArtistStats(artistName: string): Observable<FullArtistStats> {
    const stats = new FullArtistStats();
    stats.albums = [];
    stats.spotifyArtistStats = [];
    stats.tweetSummary = [];
    stats.artistGigs = [];
    stats.genre = new Genre();
    stats.genre.spotifyGivenGenre = 'rubber,dingy,rapids';
    return Observable.of(stats);
  }

  getArtistTweetWordCountForDay(artistName: string, date: string): Observable<FullArtistTweets> {
    return Observable.of(new FullArtistTweets());
  }

  getCities(): Observable<City[]> {
    const cities = [];
    cities.push(new City('Manchester'));
    cities.push(new City('Birmingham'));
    cities.push(new City('Leeds'));
    cities.push(new City('Bristol'));
    cities.push(new City('Sheffield'));
    cities.push(new City('Liverpool'));
    cities.push(new City('London'));
    cities.push(new City('Glasgow'));
    cities.push(new City('Cardiff'));
    return Observable.of(cities);
  }

  getUsersTopTracks(token: string): Observable<{}> {
    return Observable.of({});
  }
}
