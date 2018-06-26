import { TestBed, async } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { AppModule } from './app.module';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SettingsComponent } from './settings/settings.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CookieService, CookieModule } from 'ngx-cookie';
import { SpotifyTrendingComponent } from './spotify-trending/spotify-trending.component';
import { GigsOnDateComponent } from './gigs-on-date/gigs-on-date.component';
import { NewAlbumsComponent } from './new-albums/new-albums.component';
import { MainService } from './shared/main.service';
import { MusicService } from './shared/music.service';
import { FirstTimeSetupComponent } from './first-time-setup/first-time-setup.component';
import { SafeHtmlPipe } from './shared/safe-html.pipe';
import { SafeUrlPipe } from './shared/safe-url.pipe';
import { NgwWowService, NgwWowModule } from 'ngx-wow';
import { NgbActiveModal, NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FriendlyDatePipe } from './shared/friendly-date.pipe';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AgmCoreModule } from '@agm/core';
import { LoadingModule, ANIMATION_TYPES } from 'ngx-loading';
import { DragScrollModule } from 'ngx-drag-scroll';
import { ClickOutsideModule } from 'ng-click-outside';
import { StatsComponent } from './stats/stats.component';
import { ChartsModule } from 'ng2-charts';
import { AgWordCloudModule } from 'angular4-word-cloud';
import { TwitterStatsComponent } from './twitter-stats/twitter-stats.component';
import { FullDatePipe } from './shared/full-date.pipe';
import { MockMusicService } from './shared/mock-music.service';
import { MockMainService } from './shared/mock-main.service';
import { ScrollToService } from 'ng2-scroll-to-el';
import { ToastrService } from 'ngx-toastr';
import { FooterComponent } from './footer/footer.component';
import { RecommendedForYouComponent } from './recommended-for-you/recommended-for-you.component';

describe('AppComponent', () => {

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        BrowserModule,
        FormsModule,
        NgbModule.forRoot(),
        NgwWowModule.forRoot(),
        HttpModule,
        BrowserAnimationsModule,
        CookieModule.forRoot(),
        AgmCoreModule.forRoot({
          apiKey: '',
        }),
        LoadingModule,
        DragScrollModule,
        ClickOutsideModule,
        ChartsModule,
        AgWordCloudModule
      ],
      declarations: [
        AppComponent,
        DashboardComponent,
        FirstTimeSetupComponent,
        GigsOnDateComponent,
        SafeUrlPipe,
        FriendlyDatePipe,
        FullDatePipe,
        NewAlbumsComponent,
        SpotifyTrendingComponent,
        SafeHtmlPipe,
        SettingsComponent,
        StatsComponent,
        TwitterStatsComponent,
        FooterComponent,
        RecommendedForYouComponent
      ],
      providers: [
        { provide: MusicService, useClass: MockMusicService },
        { provide: MainService, useClass: MockMainService },
        { provide: NgbModal, useValue: {} },
        { provide: CookieService, useValue: { getObject: (x) => {'egg'; }} },
        { provide: ToastrService, useValue: {} },
        ScrollToService
      ]
    }).compileComponents();
  }));

  it('should create the app', async(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  }));
});
