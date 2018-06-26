import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TwitterStatsComponent } from './twitter-stats.component';
import { FullDatePipe } from '../shared/full-date.pipe';
import { AgWordCloudModule } from 'angular4-word-cloud';
import { MusicService } from '../shared/music.service';
import { MainService } from '../shared/main.service';
import { MockMusicService } from '../shared/mock-music.service';
import { MockMainService } from '../shared/mock-main.service';
import { ScrollToService } from 'ng2-scroll-to-el';
import { LoadingModule } from 'ngx-loading';

describe('TwitterStatsComponent', () => {
  let component: TwitterStatsComponent;
  let fixture: ComponentFixture<TwitterStatsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        TwitterStatsComponent,
        FullDatePipe
      ],
      imports: [
        AgWordCloudModule,
        LoadingModule
      ],
      providers: [
        { provide: MusicService, useClass: MockMusicService },
        { provide: MainService, useClass: MockMainService },
        ScrollToService
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TwitterStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
