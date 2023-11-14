import { Component, OnInit } from '@angular/core';
import { userLoggedIn } from '../Authorization/login/login.component';
import { UserService } from '../Services/user.service';
import { UserViewModel } from '../Interfaces/user-view-model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {

  isUserLoggedIn: boolean = false;
  searchString: string = '';
  users: UserViewModel[] = [];
  hideSearchUser: boolean = false;
  private _userService;
  private _router;

  constructor(userService: UserService, router: Router){
    this._userService = userService;
    this._router = router
  }

  ngOnInit(): void {
    let token = localStorage.getItem('token');
    if(token){
      this.isUserLoggedIn = true;
    }
    userLoggedIn.subscribe(response => {
      this.isUserLoggedIn = response.isLoginSuccess
    })
  }
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  searchUser(searchString: string){
    this.searchString = searchString;
    if(!this.searchString.trim()){
      this.hideSearchUser = true;
      return;
    } else{
      this.hideSearchUser = false;
    }
    this._userService.getSearchUser(this.searchString).subscribe(response => {
      if(response.isSuccess){
        this.users = response.data;
      }
    })
  }

  goToUser(userId: string){
    this._router.navigateByUrl('/').then(() => {
      this._router.navigateByUrl('/user/' + userId);
    });
    this.searchString = '';
    this.hideSearchUser = true;
  } 
}
