import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RecommendedForYouComponent } from './recommended-for-you.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SafeUrlPipe } from '../shared/safe-url.pipe';
import { MusicService } from '../shared/music.service';
import { MainService } from '../shared/main.service';
import { MockMusicService } from '../shared/mock-music.service';
import { MockMainService } from '../shared/mock-main.service';

describe('RecommendedForYouComponent', () => {
  let component: RecommendedForYouComponent;
  let fixture: ComponentFixture<RecommendedForYouComponent>;
  let helperService: MainService;
  let musicService: MusicService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        RecommendedForYouComponent,
        SafeUrlPipe
      ],
      imports: [
        NgbModule.forRoot()
      ],
      providers: [
        { provide: MusicService, useClass: MockMusicService },
        { provide: MainService, useClass: MockMainService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecommendedForYouComponent);
    component = fixture.componentInstance;
    helperService = fixture.debugElement.injector.get(MainService);
    musicService = fixture.debugElement.injector.get(MusicService);
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });
});
