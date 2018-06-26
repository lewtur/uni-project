import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpotifyTrendingComponent } from './spotify-trending.component';
import { LoadingModule } from 'ngx-loading';
import { SafeUrlPipe } from '../shared/safe-url.pipe';
import { SafeHtmlPipe } from '../shared/safe-html.pipe';
import { MusicService } from '../shared/music.service';
import { MockMusicService } from '../shared/mock-music.service';
import { MockMainService } from '../shared/mock-main.service';
import { MainService } from '../shared/main.service';
import { DetailedArtist } from '../models/detailed-artist';

describe('SpotifyTrendingComponent', () => {
  let component: SpotifyTrendingComponent;
  let fixture: ComponentFixture<SpotifyTrendingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        SpotifyTrendingComponent,
        SafeUrlPipe,
        SafeHtmlPipe
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
    fixture = TestBed.createComponent(SpotifyTrendingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set the spotify url of an artist', () => {
    // given
    const artist = new DetailedArtist();
    artist.spotifyRecordId = 'recordId';

    // when
    component.setSpotifyUrl(artist);
    const url = component.spotifyUrl;

    // then
    expect(url.startsWith('https://open.spotify.com/embed/artist/')).toBe(true);
  });
});
