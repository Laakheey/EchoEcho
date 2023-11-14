import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { userLoggedIn } from '../login/login.component';
import { AuthenticationService } from 'src/app/Services/authentication.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent implements OnInit {

  private _router;
  private _authService;
  constructor(router: Router, authService: AuthenticationService) {
    this._router = router;
    this._authService = authService;
  }

  ngOnInit(): void {
    this._authService.logout().subscribe(response =>{
      localStorage.removeItem('token');
      this._router.navigateByUrl('/auth/login');
      userLoggedIn.next({isLoginSuccess : false})
    })
  }

}
