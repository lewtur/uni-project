import { TestBed, inject } from '@angular/core/testing';

import { MainService } from './main.service';
import { CookieService } from 'ngx-cookie';

describe('MainService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        MainService,
        { provide: CookieService, useValue: { getObject: (x) => {'egg'; }} }
      ]
    });
  });

  it('should be created', inject([MainService], (service: MainService) => {
    expect(service).toBeTruthy();
  }));

  it('should return a valid color', inject([MainService], (service: MainService) => {
    expect(service.getColorForGenre('banter')).toMatch(/#[0-9A-F]{6}/);
  }));

  it('should return the same color when the same genre is requested', inject([MainService], (service: MainService) => {
    expect(service.getColorForGenre('stuff')).toBe(service.getColorForGenre('stuff'));
  }));

  it('should return different colors when the differes genres is requested', inject([MainService], (service: MainService) => {
    expect(service.getColorForGenre('stuff')).not.toBe(service.getColorForGenre('otherstuff'));
  }));
});
