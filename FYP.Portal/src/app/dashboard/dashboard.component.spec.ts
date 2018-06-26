import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardComponent } from './dashboard.component';
import { SpotifyTrendingComponent } from '../spotify-trending/spotify-trending.component';
import { NewAlbumsComponent } from '../new-albums/new-albums.component';
import { GigsOnDateComponent } from '../gigs-on-date/gigs-on-date.component';
import { FirstTimeSetupComponent } from '../first-time-setup/first-time-setup.component';
import { SafeUrlPipe } from '../shared/safe-url.pipe';
import { FriendlyDatePipe } from '../shared/friendly-date.pipe';
import { SafeHtmlPipe } from '../shared/safe-html.pipe';
import { SettingsComponent } from '../settings/settings.component';
import { MainService } from '../shared/main.service';
import { MusicService } from '../shared/music.service';
import { AppModule } from '../app.module';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModal, NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CookieService, CookieModule } from 'ngx-cookie';
import { BrowserModule } from '@angular/platform-browser';
import { NgwWowModule, NgwWowService } from 'ngx-wow';
import { HttpModule } from '@angular/http';
import { AgmCoreModule, MapsAPILoader } from '@agm/core';
import { LoadingModule, ANIMATION_TYPES } from 'ngx-loading';
import { ClickOutsideModule } from 'ng-click-outside';
import { DragScrollModule } from 'ngx-drag-scroll';
import { AppComponent } from '../app.component';
import { StatsComponent } from '../stats/stats.component';
import { TwitterStatsComponent } from '../twitter-stats/twitter-stats.component';
import { ChartsModule } from 'ng2-charts';
import { AgWordCloudModule } from 'angular4-word-cloud';
import { FullDatePipe } from '../shared/full-date.pipe';
import { ToastrService } from 'ngx-toastr';
import { NgbModalStack } from '@ng-bootstrap/ng-bootstrap/modal/modal-stack';
import { NgbModalBackdrop } from '@ng-bootstrap/ng-bootstrap/modal/modal-backdrop';
import { MockMusicService } from '../shared/mock-music.service';
import { ScrollToService } from 'ng2-scroll-to-el';
import { MockMainService } from '../shared/mock-main.service';
import { of } from 'rxjs/observable/of';
import { Constants } from '../shared/constants';
import { RecommendedForYouComponent } from '../recommended-for-you/recommended-for-you.component';

let doUserSetup = false;
const showCookieMessage = true;

const mockNgbModal = {
  open: (x) => {
    return { result: new Promise((a, b) => {}) };
  }
};

const mockCookieService = {
  getObject: (x) => {
    if (x === Constants.CookieSeenCookieKey) {
      return showCookieMessage ? null : 'something';
    } else if (x === Constants.UserPrefsCookieKey) {
      return doUserSetup ? null : 'something';
    }
  },
  putObject: (x) => {}
};

const mockToastService = {
  info: (x) => { }
};

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  const userPrefsKey = Constants.UserPrefsCookieKey;
  const cookieKey = Constants.CookieSeenCookieKey;

  let cookieService: CookieService;
  let modalService: NgbModal;
  let toastrService: ToastrService;

  beforeEach(() => {

    TestBed.configureTestingModule({
      declarations: [
        DashboardComponent,
        GigsOnDateComponent,
        SpotifyTrendingComponent,
        NewAlbumsComponent,
        StatsComponent,
        TwitterStatsComponent,
        FriendlyDatePipe,
        SafeUrlPipe,
        SafeHtmlPipe,
        FullDatePipe,
        RecommendedForYouComponent
      ],
      imports: [
        LoadingModule,
        AgmCoreModule.forRoot({
          apiKey: '',
        }),
        ChartsModule,
        AgWordCloudModule,
        NgbModule.forRoot()
      ],
      providers: [
        { provide: NgbModal, useValue: mockNgbModal },
        { provide: CookieService, useValue: mockCookieService },
        { provide: MainService, useClass: MockMainService },
        { provide: ToastrService, useValue: mockToastService },
        { provide: MusicService, useClass: MockMusicService },
        ScrollToService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;

    cookieService = fixture.debugElement.injector.get(CookieService);
    modalService = fixture.debugElement.injector.get(NgbModal);
    toastrService = fixture.debugElement.injector.get(ToastrService);

    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should query if a user has seen the cookie policy if the they are not a first time user', () => {
    // given
    doUserSetup = false;
    const cookieSpy = spyOn(cookieService, 'getObject').and.callThrough();

    // when
    fixture.detectChanges();

    // then
    expect(cookieSpy.calls.allArgs().map(x => x[0]).includes(Constants.CookieSeenCookieKey)).toBeTruthy();
  });

  it('should delay querying for the cookie policy if they are a first time user', () => {
    // given
    doUserSetup = true;
    const cookieSpy = spyOn(cookieService, 'getObject').and.callThrough();

    // when
    fixture.detectChanges();

    // then
    expect(cookieSpy.calls.allArgs().map(x => x[0]).includes(Constants.CookieSeenCookieKey)).toBeFalsy();
  });
});
