import { TestBed, inject } from '@angular/core/testing';

import { MusicService } from './music.service';
import { Http } from '@angular/http';
import { Observer } from 'rxjs/Observer';
import { Observable } from 'rxjs/Observable';

export class MockHttp {
  get(a, b): Observable<Response> {
    return Observable.of(new Response());
  }
}

describe('MusicService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        MusicService,
        { provide: Http, useClass: MockHttp }
      ]
    });
  });

  it('should be created', inject([MusicService], (service: MusicService) => {
    expect(service).toBeTruthy();
  }));
});
