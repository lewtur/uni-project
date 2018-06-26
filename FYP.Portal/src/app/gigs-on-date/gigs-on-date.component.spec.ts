import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GigsOnDateComponent } from './gigs-on-date.component';
import { LoadingModule } from 'ngx-loading';
import { FriendlyDatePipe } from '../shared/friendly-date.pipe';
import { AgmCoreModule, MapsAPILoader } from '@agm/core';
import { SafeUrlPipe } from '../shared/safe-url.pipe';
import { MusicService } from '../shared/music.service';
import { MockMusicService } from '../shared/mock-music.service';
import { CookieService } from 'ngx-cookie';
import { MainService } from '../shared/main.service';
import { MockMainService } from '../shared/mock-main.service';
import { Gig, Event, Venue} from '../models/gig';
import { DetailedArtist } from '../models/detailed-artist';
import { of } from 'rxjs/observable/of';
import { CookieXSRFStrategy } from '@angular/http';
import { UserPreferences } from '../models/user-preferences';

export function getGig(): Gig {
  const gig = new Gig();
  const artist = new DetailedArtist();
  artist.name = 'Kurupt FM';
  gig.artist = [artist];
  gig.event = new Event();
  gig.event.name = 'sick gig';
  gig.venue = new Venue();
  gig.venue.name = 'The Night Owl';

  return gig;
}

describe('GigsOnDateComponent', () => {
  let component: GigsOnDateComponent;
  let fixture: ComponentFixture<GigsOnDateComponent>;
  let musicService: MusicService;
  let cookieService: CookieService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        GigsOnDateComponent,
        FriendlyDatePipe,
        SafeUrlPipe
      ],
      imports: [
        LoadingModule,
        AgmCoreModule.forRoot({
          apiKey: '',
        })
      ],
      providers: [
        { provide: MusicService, useClass: MockMusicService },
        { provide: CookieService, useValue: { getObject: (x) => {'egg'; }} },
        { provide: MainService, useClass: MockMainService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GigsOnDateComponent);
    component = fixture.componentInstance;
    musicService = fixture.debugElement.injector.get(MusicService);
    cookieService = fixture.debugElement.injector.get(CookieService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should ask the music service for gigs from the date given', () => {
    // given
    const date = new Date(2018, 1, 1);
    component.dateToDisplay = date;
    const musicSpy = spyOn(musicService, 'getGigs').and.callThrough();

    // when
    fixture.detectChanges();

    // then
    const call = musicSpy.calls.mostRecent().args;
    expect(call[0]).toBe(date);
  });

  it('should correctly display the display gig', () => {
    // given
    const musicSpy = spyOn(musicService, 'getGigs').and.callFake(() => of ([getGig()]));

    // when
    fixture.detectChanges();

    // then
    const gig = component.displayGig;
    expect(gig.venueName).toBe('The Night Owl');
    expect(gig.eventName).toBe('sick gig');
  });

  it('should correctly order the gigs by date', () => {
    // given
    const musicSpy = spyOn(musicService, 'getGigs').and.callFake(() => {
      const gigOne = getGig();
      const gigTwo = getGig();
      gigOne.event.startDate = new Date(2018, 10, 1);
      gigOne.event.name = 'one';
      gigTwo.event.startDate = new Date(2018, 3, 1);
      gigTwo.event.name = 'two';

      return of ([gigOne, gigTwo]);
    });

    // when
    fixture.detectChanges();

    // then
    const gigs = component.relevantGigs;
    expect(gigs[0].event.name).toBe('two');
    expect(gigs[1].event.name).toBe('one');
  });

  it('should filter on gigs by user preferences', () => {
    // given
    const musicSpy = spyOn(musicService, 'getGigs').and.callFake(() => {
      const gigOne = getGig();
      const gigTwo = getGig();
      gigOne.venue.town = 'Birmingham';
      gigTwo.venue.town = 'Blackburn';

      return of ([gigOne, gigTwo]);
    });
    const cookieSpy = spyOn(cookieService, 'getObject').and.callFake(() => {
      const prefs = new UserPreferences;
      prefs.cities = ['Birmingham'];
      return prefs;
    });

    // when
    fixture.detectChanges();

    // then
    const gigs = component.relevantGigs;
    expect(gigs.length).toBe(1);
    expect(gigs[0].venue.town).toBe('Birmingham');
  });

  it('should correctly cycle through gigs', () => {
    // given
    const musicSpy = spyOn(musicService, 'getGigs').and.callFake(() => {
      const gigOne = getGig();
      const gigTwo = getGig();
      gigOne.event.name = 'one';
      gigTwo.event.name = 'two';

      return of ([gigOne, gigTwo]);
    });

    // when
    fixture.detectChanges();

    // then
    expect(component.displayGig.eventName).toBe('one');
    component.moveDayForwards();
    expect(component.displayGig.eventName).toBe('two');
  });
});
