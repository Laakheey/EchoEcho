import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { loginViewModel } from 'src/app/Interfaces/loginViewModel';
import { signupResponseViewModel } from 'src/app/Interfaces/signupViewModel';
import { AuthenticationService } from 'src/app/Services/authentication.service';

import {MessageService} from 'primeng/api'
import { Router } from '@angular/router';
import { Subject } from 'rxjs';

export const userLoggedIn = new Subject<{isLoginSuccess: boolean}>();

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [MessageService]
})
export class LoginComponent implements OnInit {

  private _authService;
  private _messageService;
  private _router;

  constructor(authService: AuthenticationService, messageService:MessageService, router: Router) {
    this._authService = authService;
    this._messageService = messageService;
    this._router = router;
  }

  ngOnInit(): void {
    this.loginFormValue
  }

  isLoginSuccess: boolean = true;
  loginFormValue!: loginViewModel;

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.email, Validators.required]),
    password: new FormControl('', [Validators.required]),
  }); 

  isSubmitted: boolean = false;
  login(){
    if(!this.loginForm.valid){
      this.isSubmitted = true;
      return
    }
    this.loginFormValue = this.loginForm.value as loginViewModel;
    this._authService.login(this.loginFormValue).subscribe((response:signupResponseViewModel)=>{
      if(response.isSuccess){
        localStorage.setItem('token', response.token)
        this._router.navigateByUrl('/');
      } else{
        this.isLoginSuccess = false;
      }
      userLoggedIn.next({
        isLoginSuccess : this.isLoginSuccess
      });
    });
  }

  socialMedia(loginMedia: string){
    alert(`Login with ${loginMedia} will be implemented soon`);
  }


}
