import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from './Services/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'app';
  loginUserId: string = '';
  isRefreshToken: boolean = false;
  constructor(private authService: AuthenticationService) { }

  ngOnInit(): void {
    this.getLoggedInUserDetails();
    if(this.isRefreshToken){
      this.authService.refreshToken(this.loginUserId).subscribe(response => {
        localStorage.setItem('token', response.token);
        window.location.reload();
        this.isRefreshToken = false;
      });
    }
  }

  getLoggedInUserDetails() {
    var validToken = localStorage.getItem('token');
    if (validToken != null) {
      let jwtData = validToken.split('.')[1];
      let decodedJwtJsonData = window.atob(jwtData);
      let decodedJwtData = JSON.parse(decodedJwtJsonData);
      this.loginUserId = decodedJwtData.jti;
      if (decodedJwtData.auth_time) {
        let authTimeDate = new Date(decodedJwtData.auth_time);
        let authTimeMilliseconds = authTimeDate.getTime();
        let currentDateTime = new Date();
        let fiveHoursAgo = new Date(currentDateTime.getTime() - (5 * 60 * 60 * 1000));
        if (authTimeMilliseconds < fiveHoursAgo.getTime() || authTimeDate.getDate() !== currentDateTime.getDate()) {
          this.isRefreshToken = true;
        } else {
          this.isRefreshToken = false;
        }
      }
    }
  }

}
