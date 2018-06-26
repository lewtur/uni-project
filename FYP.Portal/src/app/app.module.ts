import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { HttpModule } from '@angular/http';
import { MusicService } from './shared/music.service';
import { DashboardComponent } from './dashboard/dashboard.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FirstTimeSetupComponent } from './first-time-setup/first-time-setup.component';
import { NgwWowModule } from 'ngx-wow';
import { CookieModule } from 'ngx-cookie';
import { GigsOnDateComponent } from './gigs-on-date/gigs-on-date.component';
import { AgmCoreModule } from '@agm/core';
import { SafeUrlPipe } from './shared/safe-url.pipe';
import { FriendlyDatePipe } from './shared/friendly-date.pipe';
import { LoadingModule, ANIMATION_TYPES  } from 'ngx-loading';
import { ANALYZE_FOR_ENTRY_COMPONENTS } from '@angular/core/src/metadata/di';
import { MainService } from './shared/main.service';
import { NewAlbumsComponent } from './new-albums/new-albums.component';
import { DragScrollModule } from 'ngx-drag-scroll';
import { SpotifyTrendingComponent } from './spotify-trending/spotify-trending.component';
import { SafeHtmlPipe } from './shared/safe-html.pipe';
import { SettingsComponent } from './settings/settings.component';
import { ClickOutsideModule } from 'ng-click-outside';
import { ChartsModule } from 'ng2-charts';
import { StatsComponent } from './stats/stats.component';
import { AgWordCloudModule } from 'angular4-word-cloud';
import { TwitterStatsComponent } from './twitter-stats/twitter-stats.component';
import { FullDatePipe } from './shared/full-date.pipe';
import { ScrollToModule } from 'ng2-scroll-to-el';
import { FooterComponent } from './footer/footer.component';
import { ToastrModule } from 'ngx-toastr';
import { RecommendedForYouComponent } from './recommended-for-you/recommended-for-you.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    FirstTimeSetupComponent,
    GigsOnDateComponent,
    SafeUrlPipe,
    FriendlyDatePipe,
    NewAlbumsComponent,
    SpotifyTrendingComponent,
    SafeHtmlPipe,
    SettingsComponent,
    StatsComponent,
    TwitterStatsComponent,
    FullDatePipe,
    FooterComponent,
    RecommendedForYouComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    NgbModule.forRoot(),
    NgwWowModule.forRoot(),
    HttpModule,
    BrowserAnimationsModule,
    CookieModule.forRoot(),
    AgmCoreModule.forRoot({
      apiKey: '***REMOVED***',
    }),
    LoadingModule.forRoot({
      animationType: ANIMATION_TYPES.circleSwish,
      primaryColour: '#000000',
      backdropBackgroundColour: '#fff'
    }),
    DragScrollModule,
    ClickOutsideModule,
    ChartsModule,
    AgWordCloudModule.forRoot(),
    ScrollToModule.forRoot(),
    ToastrModule.forRoot()
  ],
  providers: [
    MusicService,
    MainService
  ],
  entryComponents: [
    FirstTimeSetupComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
