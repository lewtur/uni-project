import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FirstTimeSetupComponent } from './first-time-setup.component';
import { NgbModal, NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { MusicService } from '../shared/music.service';
import { MockMusicService } from '../shared/mock-music.service';
import { NgwWowService } from 'ngx-wow';
import { CookieService } from 'ngx-cookie';
import { MainService } from '../shared/main.service';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { City } from '../models/city';
import { Constants } from '../shared/constants';

const mockCookieService = {
  getObject: (x) => x,
  putObject: (x, y, z) => {}
};

describe('FirstTimeSetupComponent', () => {
  let component: FirstTimeSetupComponent;
  let fixture: ComponentFixture<FirstTimeSetupComponent>;
  let cookieService: CookieService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FirstTimeSetupComponent ],
      imports: [ NoopAnimationsModule ],
      providers: [
        { provide: NgbActiveModal, useValue: {} },
        { provide: MusicService, useClass: MockMusicService },
        { provide: NgwWowService, useValue: {init: () => {} } },
        { provide: CookieService, useValue: mockCookieService },
        MainService
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FirstTimeSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    cookieService = fixture.debugElement.injector.get(CookieService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should update the user preferences when the user clicks next', () => {
    // given
    const city = new City('Birmingham');
    city.selected = true;
    component.cities = [city];

    // when
    const cookieSpy = spyOn(cookieService, 'putObject').and.callThrough();
    component.clickNext();

    // then
    const cookieArguments = cookieSpy.calls.mostRecent().args;
    expect(cookieArguments[0]).toBe(Constants.UserPrefsCookieKey);
    expect(cookieArguments[1].cities[0]).toBe('Birmingham');
  });
});
