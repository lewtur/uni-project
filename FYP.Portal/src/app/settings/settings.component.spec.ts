import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsComponent } from './settings.component';
import { MusicService } from '../shared/music.service';
import { MockMusicService } from '../shared/mock-music.service';
import { CookieService } from 'ngx-cookie';
import { MainService } from '../shared/main.service';
import { MockMainService } from '../shared/mock-main.service';
import { City } from '../models/city';
import { of } from 'rxjs/observable/of';
import { SafeUrlPipe } from '../shared/safe-url.pipe';

describe('SettingsComponent', () => {
  let component: SettingsComponent;
  let fixture: ComponentFixture<SettingsComponent>;
  let cookieService: CookieService;
  let musicService: MusicService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        SettingsComponent,
        SafeUrlPipe
      ],
      providers: [
        { provide: MusicService, useClass: MockMusicService },
        { provide: CookieService, useValue: { getObject: (x) => {'egg'; }, putObject: (x) => {}} },
        { provide: MainService, useClass: MockMainService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsComponent);
    component = fixture.componentInstance;
    cookieService = fixture.debugElement.injector.get(CookieService);
    musicService = fixture.debugElement.injector.get(MusicService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should update the preferences cookie when a city is selected', () => {
    // given
    const cookieSpy = spyOn(cookieService, 'putObject').and.callThrough();
    spyOn(musicService, 'getCities').and.callFake(() => {
      return of([new City('Burnley'), new City('Fulham'), new City('Barbados')]);
    });

    // when
    fixture.detectChanges();
    component.selectCity(component.cities[2]);

    // then
    const cookieArgs = cookieSpy.calls.mostRecent().args;
    expect(cookieSpy).toHaveBeenCalledTimes(1);
    expect(cookieArgs[1].cities[0]).toBe('Barbados');
  });
});
