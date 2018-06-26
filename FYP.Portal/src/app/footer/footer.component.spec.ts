import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FooterComponent } from './footer.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

const mockNgbModal = {
  open: (x) => {
    return { result: new Promise((a, b) => {}) };
  }
};

describe('FooterComponent', () => {
  let component: FooterComponent;
  let fixture: ComponentFixture<FooterComponent>;
  let modalService: NgbModal;


  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ FooterComponent ],
      providers: [
        { provide: NgbModal, useValue: mockNgbModal }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FooterComponent);
    component = fixture.componentInstance;
    modalService = fixture.debugElement.injector.get(NgbModal);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture).toBeTruthy();
  });

  it('should open the policy modal when the user clicks it', () => {
    // given
    const modalSpy = spyOn(modalService, 'open').and.callThrough();

    // when
    component.openModal(null);

    // then
    expect(modalSpy.calls.count()).toBe(1);
  });

});
