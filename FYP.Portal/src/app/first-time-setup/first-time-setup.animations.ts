import { trigger, state, style, transition, animate, keyframes } from '@angular/animations';

export const SetupAnimations = [
  trigger('fadeOut', [
    state('true', style({
      opacity: 0,
      display: 'none'
    })),
    transition('* => true', animate('150ms ease-out')),
  ]),
  trigger('fadeIn', [
    state('true', style({
      opacity: 1
    })),
    transition('* => true', animate('950ms ease-in')),
  ]),
  trigger('oneCitySelected', [
    state('true', style({
      backgroundColor: '#cc4218',
      color: 'white'
    })),
    state('false', style({
      backgroundColor: '#094074',
      color: 'white'
    })),
    transition('true <=> false', animate('350ms cubic-bezier(1.000, 0.245, 0.285, 0.610)')),
  ]),
  trigger('atLeastOneCitySelected', [
    state('true', style({
      opacity: 1
    })),
    state('false', style({
      opacity: 0
    })),
    transition('false => true', animate('350ms cubic-bezier(1.000, 0.245, 0.285, 0.610)')),
  ]),
  trigger('movedOnToGenres-Genres', [
    state('true', style({
      opacity: 0
    })),
    state('false', style({
      opacity: 1
    })),
    transition('true <=> false', animate('350ms cubic-bezier(1.000, 0.245, 0.285, 0.610)')),
  ]),
  trigger('movedOnToGenres-Desc', [
    transition('true <=> false', animate(500, keyframes([
      style({opacity: 1}),
      style({opacity: 0}),
      style({opacity: 1}) ])))
  ])
];
