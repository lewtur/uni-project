import { FriendlyDatePipe } from './friendly-date.pipe';

describe('FriendlyDatePipe', () => {
  it('should create an instance', () => {
    const pipe = new FriendlyDatePipe();
    expect(pipe).toBeTruthy();
  });

  it('should return \'Tonight\' when the date is now', () => {
    const pipe = new FriendlyDatePipe();
    expect(pipe.transform(new Date())).toBe('Tonight');
  });

  it('should return \'Tomorrow\' when the date is tomorrow', () => {
    const pipe = new FriendlyDatePipe();
    const date = new Date();
    date.setDate(date.getDate() + 1);

    expect(pipe.transform(date)).toBe('Tomorrow');
  });
});
