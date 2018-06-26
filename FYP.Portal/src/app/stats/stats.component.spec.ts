import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StatsComponent } from './stats.component';
import { LoadingModule } from 'ngx-loading';
import { ChartsModule } from 'ng2-charts';
import { AgWordCloudModule } from 'angular4-word-cloud';
import { TwitterStatsComponent } from '../twitter-stats/twitter-stats.component';
import { FullDatePipe } from '../shared/full-date.pipe';
import { MainService } from '../shared/main.service';
import { MockMainService } from '../shared/mock-main.service';
import { MusicService } from '../shared/music.service';
import { MockMusicService } from '../shared/mock-music.service';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ScrollToService } from 'ng2-scroll-to-el';
import { FullArtistStats, SpotifyArtistStats, TwitterDaySummary, EventSummary } from '../models/full-artist-stats';
import { of } from 'rxjs/observable/of';
import { HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';

let today: Date;

describe('StatsComponent', () => {
  let component: StatsComponent;
  let fixture: ComponentFixture<StatsComponent>;
  let musicService: MusicService;
  let helperService: MainService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        StatsComponent,
        TwitterStatsComponent,
        FullDatePipe
      ],
      imports: [
        LoadingModule,
        ChartsModule,
        AgWordCloudModule
      ],
      providers: [
        { provide: MusicService, useClass: MockMusicService },
        { provide: MainService, useClass: MockMainService },
        { provide: NgbModal, useValue: {} },
        ScrollToService
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatsComponent);
    component = fixture.componentInstance;
    musicService = fixture.debugElement.injector.get(MusicService);
    helperService = fixture.debugElement.injector.get(MainService);
    today = new Date();
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should correctly add the spotify stats the the graph', () => {
    // given
    spyOn(musicService, 'getArtistStats').and.callFake(getFakeSpotifyStats);

    // when
    fixture.detectChanges();
    helperService.userClickedRecommendedArtist.next('swag');
    const list = component.lineChartData;
    console.log(list);

    // then
    expect(list[0].data.length).toBe(2);
    expect(list[0].label).toBe('Spotify Popularity');
    expect(list[0].data[0].y).toBe(12);
  });

  it('should correctly add tweet data to the graph', () => {
    // given
    spyOn(musicService, 'getArtistStats').and.callFake(getFakeTwitterStats);

    // when
    fixture.detectChanges();
    helperService.userClickedRecommendedArtist.next('swag');
    const list = component.lineChartData;

    // then
    expect(list[0].data.length).toBe(2);
    expect(list[0].data[0].t).toBe(today);
    expect(list[0].data[1].y).toBe(34);
  });

  it ('should correctly add gigs to the graph', () => {
    // given
    spyOn(musicService, 'getArtistStats').and.callFake(getFakeGigs);

    // when
    fixture.detectChanges();
    helperService.userClickedRecommendedArtist.next('swag');
    const list = component.lineChartData;

    // then
    expect(list[0].data.length).toBe(1);
    expect(list[0].label).toBe('The Night Owl, Birmingham');
  });
});

const getFakeSpotifyStats = () => {
  const stats = new FullArtistStats();
  stats.spotifyArtistStats = [];

  const stat1 = new SpotifyArtistStats();
  stat1.popularity = 12;
  stats.spotifyArtistStats.push(stat1);

  const stat2 = new SpotifyArtistStats();
  stat2.popularity = 34;
  stats.spotifyArtistStats.push(stat2);

  return of (stats);
};

const getFakeTwitterStats = () => {
  const stats = new FullArtistStats();
  stats.tweetSummary = [];

  const stat1 = new TwitterDaySummary();
  stat1.percentage = 50;
  stat1.date = today;
  stats.tweetSummary.push(stat1);

  const stat2 = new TwitterDaySummary();
  stat2.percentage = 34;
  const yesterday = today;
  yesterday.setDate(yesterday.getDate() - 1);
  stat2.date = yesterday;
  stats.tweetSummary.push(stat2);

  return of (stats);
};

const getFakeGigs = () => {
  const stats = new FullArtistStats();
  stats.artistGigs = [];

  const event1 = new EventSummary();
  event1.venueName = 'The Night Owl';
  event1.venueLocation = 'Birmingham';
  stats.artistGigs.push(event1);

  return of (stats);
};
