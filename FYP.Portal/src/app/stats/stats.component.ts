import { Component, OnInit, Input, ViewChild, ElementRef, SimpleChanges } from '@angular/core';
import { MusicService } from '../shared/music.service';
import { FullArtistStats, SpotifyArtistStats } from '../models/full-artist-stats';
import { moment } from '../shared/moment';
import { NgbTooltipConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChartOptions, ChartColors } from './stats.utils';
import { MainService } from '../shared/main.service';
import { AgWordCloudData } from 'angular4-word-cloud';
import { Observer } from 'rxjs/Observer';
import { Observable } from 'rxjs/Observable';
import { AgWordCloudDirective } from 'angular4-word-cloud';
import { DragScrollDirective } from 'ngx-drag-scroll';
import { Constants } from '../shared/constants';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.scss']
})
export class StatsComponent implements OnInit {

  @ViewChild(BaseChartDirective) chart: BaseChartDirective;

  artistName: string;

  fullStats: FullArtistStats;
  loading = true;
  genreList: string[];
  dateSelected: Date;
  loadingTwitterData = false;

  lineChartData: Array<any> = [];
  lineChartType = 'line';
  lineChartLabels: Array<any> = [];
  lineChartColors = ChartColors;
  lineChartOptions = ChartOptions;

  constructor(
    private musicService: MusicService,
    private modalService: NgbModal,
    private helperService: MainService
  ) {

    ChartOptions.onClick = (event, elements) => {
      if (!elements || !elements.length) {
        return;
      }

      const jsonDate = `Date(${elements[1]._xScale._timestamps.datasets[elements[1]._datasetIndex][elements[1]._index]})`;
      this.dateSelected = moment(jsonDate).toDate();
      const dateString = moment(jsonDate).format('YYYY-MM-DD');

      this.loadingTwitterData = true;
      this.musicService.getArtistTweetWordCountForDay(this.artistName, dateString).subscribe(tweetData => {
        this.loadingTwitterData = false;
        this.helperService.updateTweetData(tweetData);
      });
    };
  }

  ngOnInit() {
    this.helperService.userClickedRecommendedArtist.subscribe(artistName => {
      this.artistName = artistName;
      this.musicService.getArtistStats(this.artistName).subscribe(value => {
        this.fullStats = value;
        this.lineChartData.length = 0;
        this.lineChartLabels.length = 0;

        if (this.fullStats.spotifyArtistStats) {
          const spotifyData = [];
          this.lineChartLabels.push(...this.fullStats.spotifyArtistStats.map(x => x.datePosted));

          this.fullStats.spotifyArtistStats.forEach(stat => {
            spotifyData.push({t: stat.datePosted, y: stat.popularity});
          });

          this.lineChartData.push({
            data: spotifyData,
            label: 'Spotify Popularity'
          });
        }

        if (this.fullStats.tweetSummary) {
          const tweetData = [];
          this.fullStats.tweetSummary.forEach(tweet => {
            tweetData.push({t: tweet.date, y: tweet.percentage, tweetCount: tweet.tweetCount});
          });

          this.lineChartData.push({
            data: tweetData,
            label: 'Twitter Popularity',
            isTwitter: true
          });
        }

        if (this.fullStats.artistGigs && this.fullStats.artistGigs.length) {
          this.fullStats.artistGigs.forEach((gig, index) => {
            this.lineChartData.push({
              data: [{t: gig.startDate, y: this.findNearestPopularity(gig.startDate)}],
              label: `${gig.venueName}, ${gig.venueLocation}`,
              pointRadius: 10,
              pointHoverRadius: 10,
              pointBackgroundColor: 'rgba(204,66,24,0.4)',
              isGig: true
            });
          });
        }

        if (this.fullStats.albums && this.fullStats.albums.length) {
          this.fullStats.albums.forEach(album => {
            this.lineChartData.push({
              data: [{t: album.releaseDate, y: this.findNearestPopularity(album.releaseDate)}],
              label: 'Album: ' + album.name,
              pointRadius: 10,
              pointHoverRadius: 10,
              pointBackgroundColor: 'rgba(17, 167, 159,0.5)',
              isGig: true
            });
          });
        }

        if (this.fullStats.genre) {
          if (this.fullStats.genre.spotifyGivenGenre) {
            this.genreList = this.fullStats.genre.spotifyGivenGenre.split(',');
          } else if (this.fullStats.genre.otherGenresGivenInRelatedArtists) {
            this.genreList = this.fullStats.genre.otherGenresGivenInRelatedArtists.split(',');
          }

          this.genreList = this.genreList.slice(0, 4);
        }

        if (this.chart) {
          this.chart.ngOnChanges({} as SimpleChanges);
        }
        this.loading = false;
      });
    });

  }

  findNearestPopularity(date: Date): number {
    let candidates: SpotifyArtistStats = null;
    let index = 0;

    if (!this.fullStats.spotifyArtistStats) {
      return 0;
    }

    while (!candidates) {
      candidates = this.fullStats.spotifyArtistStats.find(x => Math.abs(moment(x.datePosted).diff(moment(date), 'days')) === index);
      index++;
    }

    return candidates.popularity;
  }

  openHelper(content) {
    this.modalService.open(content, { windowClass: 'dark-modal' });
  }

  getGenreColor(genre: string): string {
    return this.helperService.getColorForGenre(genre);
  }
}
