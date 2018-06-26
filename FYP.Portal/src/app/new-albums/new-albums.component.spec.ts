import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewAlbumsComponent } from './new-albums.component';
import { LoadingModule } from 'ngx-loading';
import { SafeUrlPipe } from '../shared/safe-url.pipe';
import { MusicService } from '../shared/music.service';
import { MockMusicService } from '../shared/mock-music.service';
import { MainService } from '../shared/main.service';
import { MockMainService } from '../shared/mock-main.service';
import { Album } from '../models/album';
import { of } from 'rxjs/observable/of';

describe('NewAlbumsComponent', () => {
  let component: NewAlbumsComponent;
  let fixture: ComponentFixture<NewAlbumsComponent>;
  let musicService: MusicService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        NewAlbumsComponent,
        SafeUrlPipe
      ],
      imports: [
        LoadingModule
      ],
      providers: [
        { provide: MusicService, useClass: MockMusicService },
        { provide: MainService, useClass: MockMainService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewAlbumsComponent);
    component = fixture.componentInstance;
    musicService = fixture.debugElement.injector.get(MusicService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should split the genres of an album correctly', () => {
    // given
    spyOn(musicService, 'getAlbums').and.callFake(() => {
      const album = new Album();
      album.genres = 'swag,swagger,swaggier,swaggest';
      return of ([album]);
    });

    // when
    fixture.detectChanges();
    const genres: any[] = (<any>(component.albums[0])).genreList;

    // then
    expect(genres.length).toBe(4);
    expect(genres[0]).toBe('swag');
  });
});
