import { Component, OnInit } from '@angular/core';
import { Artist } from '../models/artist';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FirstTimeSetupComponent } from '../first-time-setup/first-time-setup.component';
import { Observable } from 'rxjs/Observable';
import { CookieService } from 'ngx-cookie';
import { Constants } from '../shared/constants';
import { MainService } from '../shared/main.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  cookieMessage = `
    Like every other site on the internet, we use cookies to provide a better service. You can read
    more about them about the bottom of this page (if you want). Thanks!
  `;

  today: Date = new Date();

  constructor(
    private modalService: NgbModal,
    private cookieService: CookieService,
    private mainService: MainService,
    private toastr: ToastrService,
    private helperService: MainService) { }

  ngOnInit() {
    const prefs = this.cookieService.getObject(Constants.UserPrefsCookieKey);
    if (!prefs) {
      setTimeout(() => {
        this.modalService.open(FirstTimeSetupComponent, {size: 'lg'}).result.then(r => {
          this.mainService.updatePreferencesFromCookie();
          this.openCookieToastIfNotSeen();
        });
      }, 0);
    } else {
      if (!this.cookieService.getObject(Constants.CookieSeenCookieKey)) {
        setTimeout(() => this.openCookieToastIfNotSeen(), 1000);
      }
    }

    const params = this.getParams(window.location.hash);
    let token = params['access_token'];
    if (token) {
      localStorage.setItem(Constants.SpotifyAccessTokenKey, token);
    } else {
      token = localStorage.getItem(Constants.SpotifyAccessTokenKey);
    }
    setTimeout(() => this.helperService.updateSpotifyToken(token), 0);
  }

  openCookieToastIfNotSeen(): void {
    if (!this.cookieService.getObject(Constants.CookieSeenCookieKey)) {
      const expiryDate = new Date();
      expiryDate.setFullYear(expiryDate.getFullYear() + 1);
      setTimeout(() => {
        this.toastr.info(this.cookieMessage, 'Cookie Policy', {
          disableTimeOut: true,
          closeButton: true,
          positionClass: 'toast-bottom-center',
          enableHtml: true,
          toastClass: 'toast cookie-toast'
        }).onHidden.subscribe(x => {
          this.cookieService.putObject(Constants.CookieSeenCookieKey, 'yup', {expires: expiryDate});
        });
      }, 1000);
    }
  }

  getParams(query) {
    if (!query) {
      return {};
    }

    return (/^[?#]/.test(query) ? query.slice(1) : query)
      .split('&')
      .reduce((params, param) => {
        const [key, value] = param.split('=');
        params[key] = value ? decodeURIComponent(value.replace(/\+/g, ' ')) : '';
        return params;
      }, {});
  }

}
