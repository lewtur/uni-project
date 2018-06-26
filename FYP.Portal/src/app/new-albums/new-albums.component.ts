import { Component, OnInit, ViewChild } from '@angular/core';
import { MusicService } from '../shared/music.service';
import { Album } from '../models/album';
import { DragScrollDirective } from 'ngx-drag-scroll';
import { MainService } from '../shared/main.service';

@Component({
  selector: 'app-new-albums',
  templateUrl: './new-albums.component.html',
  styleUrls: ['./new-albums.component.scss']
})
export class NewAlbumsComponent implements OnInit {

  @ViewChild('nav', {read: DragScrollDirective}) ds: DragScrollDirective;

  albums: Album[];
  loading = true;
  spotifyAlbumUrl: string;
  showCycleButtons = false;
  albumsReleased = false;

  constructor(
    private musicService: MusicService,
    private helperService: MainService
  ) { }

  ngOnInit() {
    this.musicService.getAlbums(new Date()).subscribe(value => {
      this.albums = value;
      this.showCycleButtons = this.albums.length > 4;
      this.albumsReleased = this.albums && !!this.albums.length;

      this.albums.forEach(x => {
        (<any>x).genreList =  x.genres.split(',').slice(0, 4);
      });

      if (this.ds) {
        this.ds.snapDisabled = true;
      }

      this.loading = false;
    });
  }

  playAlbum(album: Album): void {
    this.spotifyAlbumUrl = `https://open.spotify.com/embed/album/${album.spotifyRecordId}`;
  }

  moveLeft(): void {
    this.ds.moveLeft();
  }

  moveRight(): void {
    this.ds.moveRight();
  }

  getGenreColor(genre: string): string {
    return this.helperService.getColorForGenre(genre);
  }
}
