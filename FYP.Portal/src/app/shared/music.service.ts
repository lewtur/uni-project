import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { Artist } from '../models/artist';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/delay';
import { environment } from '../../environments/environment';
import { City } from '../models/city';
import { Genre } from '../models/genre';
import { Gig } from '../models/gig';
import { IMenuItem } from '../models/menu-item';
import { moment } from '../shared/moment';
import { Album } from '../models/album';
import { DebugContext } from '@angular/core/src/view';
import { DetailedArtist } from '../models/detailed-artist';
import { FullArtistStats } from '../models/full-artist-stats';
import { AgWordCloudData } from 'angular4-word-cloud';
import { FullArtistTweets } from '../models/full-artist-tweets';
import { UserRecommendedArtist } from '../models/artist-with-features';
import { of } from 'rxjs/observable/of';

@Injectable()
export class MusicService {

  constructor(private http: Http) { }

  getPopularArtists(daysToLookBack: number, limit: number): Observable<DetailedArtist[]> {
    const url = `${environment.serviceUrl}/Artist/MostPopular?DaysToLookBack=${daysToLookBack}&Limit=${limit}`;
    const headers = new Headers({'Content-Type': 'application/json'});

    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() as DetailedArtist[])
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }

  getGigs(date: Date): Observable<Gig[]> {
    const startDate = moment(date).format('YYYY-MM-DD');
    const endDate = moment(date).add(7, 'days').format('YYYY-MM-DD');

    const url = `${environment.serviceUrl}/Event?StartDate=${startDate}&EndDate=${endDate}`;
    const headers = new Headers({'Content-Type': 'application/json'});

    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() as Gig)
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }

  getAlbums(date: Date): Observable<Album[]> {
    const dateParam = moment(date).format('YYYY-MM-DD');

    const url = `${environment.serviceUrl}/Album/ByReleaseDate?Date=${dateParam}`;
    const headers = new Headers({'Content-Type': 'application/json'});

    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() as Album[])
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }

  getArtists(query: string): Observable<Artist[]> {
    if (!query) {
      return Observable.of(null);
    }

    const url = `${environment.serviceUrl}/Artist?Term=${query}&Page=1&PageSize=10`;
    const headers = new Headers({'Content-Type': 'application/json'});
    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json().artists as Artist[])
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }

  getArtistStats(artistName: string): Observable<FullArtistStats> {
    const url = `${environment.serviceUrl}/Artist/Stats?ArtistName=${encodeURIComponent(artistName)}`;
    const headers = new Headers({'Content-Type': 'application/json'});

    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() as FullArtistStats)
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }

  getArtistTweetWordCountForDay(artistName: string, date: string): Observable<FullArtistTweets> {
    const url = `${environment.serviceUrl}/Twitter?ArtistName=${encodeURIComponent(artistName)}&Date=${date}`;
    const headers = new Headers({'Content-Type': 'application/json'});

    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() as FullArtistTweets)
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }

  getUsersTopTracks(token: string): Observable<{}> {
    const url = `https://api.spotify.com/v1/me/top/artists/?time_range=short_term`;
    const headers = new Headers({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + token
    });

    return this.http.get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() )
      .catch(x => of ({failed: true}));
  }

  getUsersSuggestedArtists(genres: string): Observable<UserRecommendedArtist[]> {
    const url = `${environment.serviceUrl}/Artist/GetUsersTrendingArtist?Genres=${genres}`;
    const headers = new Headers({'Content-Type': 'application/json'});

    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() as UserRecommendedArtist[])
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }

  getCities(): Observable<City[]> {
    const url = `${environment.serviceUrl}/Event/Cities`;
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
    return Observable.of(cities).catch(error => {
      console.error(error);
      return Observable.throw(error.message || error);
    });
    // return this.getMenuOption<City>(url);
  }

  getGenres(): Observable<Genre[]> {
    const url = `${environment.serviceUrl}/Event/Genres`;
    const genres = [];
    genres.push(new Genre('Rock'));
    genres.push(new Genre('Indie'));
    genres.push(new Genre('Soul'));
    genres.push(new Genre('Jazz'));
    genres.push(new Genre('Classical'));
    genres.push(new Genre('Ska'));
    genres.push(new Genre('Reggae'));
    genres.push(new Genre('Dance'));
    genres.push(new Genre('Electronic'));
    return Observable.of(genres).catch(error => {
      console.error(error);
      return Observable.throw(error.message || error);
    });
    // return this.getMenuOption<Genre>(url);
  }

  getMenuOption<T extends IMenuItem>(url: string): Observable<T[]> {
    const headers = new Headers({'Content-Type': 'application/json'});
    return this.http
      .get(url, new RequestOptions({headers: headers}))
      .map(response => response.json() as string[])
      .map(array => {
        const values = [];
        array.forEach(x => {
          const a: T = {name: x, selected: false} as T;
          values.push(a);
        });
        return values;
      })
      .catch(error => {
        console.error(error);
        return Observable.throw(error.message || error);
      });
  }
}
