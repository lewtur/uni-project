import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { AgWordCloudDirective } from 'angular4-word-cloud';
import { DragScrollDirective } from 'ngx-drag-scroll';
import { MainService } from '../shared/main.service';
import { ScrollToService } from 'ng2-scroll-to-el';

@Component({
  selector: 'app-twitter-stats',
  templateUrl: './twitter-stats.component.html',
  styleUrls: ['./twitter-stats.component.scss']
})
export class TwitterStatsComponent implements OnInit {

  @ViewChild('word_cloud_chart') word_cloud_chart: AgWordCloudDirective;
  @ViewChild('nav', {read: DragScrollDirective}) ds: DragScrollDirective;
  @ViewChild('nav') private scroller: ElementRef;
  @ViewChild('title') title: ElementRef;

  @Input() artistName: string;
  @Input() date: Date;
  @Input() loading = false;

  wordData = [];
  tweets = [];
  wordCloudWidth = 0;
  scrollingRight = true;
  stopScrolling = false;

  wordOptions = {
    settings: {
      minFontSize: 25,
      maxFontSize: 35,
    },
    margin: {
      top: 10,
      right: 10,
      bottom: 10,
      left: 10
    },
    labels: false
  };

  constructor(private helperService: MainService, private scrollService: ScrollToService) { }

  ngOnInit() {
    this.wordCloudWidth = window.innerWidth * 0.7;

    if (this.ds) {
      this.ds.snapDisabled = true;
    }

    window.onresize = (e) => {
      this.wordCloudWidth = window.innerWidth * 0.7;
      this.word_cloud_chart.update();
    };

    setInterval(() => {
      const element = this.scroller.nativeElement;

      if (!element.classList.value.includes('stop-scrolling')) {
        if (window.innerWidth <= 576) {
          return;
        }

        if (this.scrollingRight) {
          element.scrollLeft += 2;

          if (element.offsetWidth + element.scrollLeft === element.scrollWidth) {
            this.scrollingRight = false;
          }
        } else {
          element.scrollLeft -= 2;
          if (element.scrollLeft <= 0) {
            this.scrollingRight = true;
          }
        }
      }
    }, 100);

    this.helperService.tweetDataChanged.subscribe(tweetData => {
      this.wordData = tweetData.wordPairings;
      this.tweets = tweetData.tweets;
      setTimeout(() => {
        this.word_cloud_chart.update();
        this.scrollService.scrollTo(this.title.nativeElement);
      });
      this.scroller.nativeElement.scrollLeft = 0;
      this.scrollingRight = true;
    });

    this.helperService.userClickedRecommendedArtist.subscribe(x => {
      this.wordData = [];
    });
  }

  openLink(link: string) {
    if (!link) {
      return;
    }

    window.open(link, '_blank');
  }
}
