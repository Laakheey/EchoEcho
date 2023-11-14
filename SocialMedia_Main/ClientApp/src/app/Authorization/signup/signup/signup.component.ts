import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { AuthenticationService } from 'src/app/Services/authentication.service';

import {genderType, signupResponseViewModel, signupViewModel} from 'src/app/Interfaces/signupViewModel';

import { MessageService } from 'primeng/api'
import { Router } from '@angular/router';
import { PlaceService } from 'src/app/Services/place.service';
import { CountryViewModel } from 'src/app/Interfaces/country-view-model';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css'],
  providers: [MessageService]
})
export class SignupComponent implements OnInit {

  private _authService;
  private _messageService;
  private _router;
  private _placeService;

  signupFormValue!: signupViewModel;
  isSubmitted: boolean = false;
  isPasswordMatch: boolean = true;
  countries:CountryViewModel[] = [];
  cities:string[] = [];
  password: string = 'password';
  confirmPassword: string = 'password';
  pass: boolean = false;
  confirmPass: boolean = false;

  constructor(authService: AuthenticationService, placeService: PlaceService, messageService: MessageService, router: Router){
    this._authService = authService;
    this._messageService = messageService;
    this._router = router;
    this._placeService = placeService;
  }

  ngOnInit(): void {
    this.signupFormValue;
    this._placeService.getCountries().subscribe(response => {
      this.countries = response;
    });
  }

  signupForm = new FormGroup({
    firstName : new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(15)]),
    lastName : new FormControl('', []),
    email : new FormControl('', [Validators.required, Validators.email]),
    password : new FormControl('', [Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$/)]),
    confirmPassword: new FormControl('', [Validators.required,] ),
    phoneNumber : new FormControl('', [Validators.required, Validators.pattern(/^\d{10}$/)]),
    dateOfBirth : new FormControl('', [Validators.required]),
    gender : new FormControl('', [Validators.required]),
    country: new FormControl('', [Validators.required]),
    city : new FormControl('', [Validators.required]),
  });

  genderEnumToString = {
    [genderType.Male]: 'Male',
    [genderType.Female]: 'Female',
    [genderType.Unknown]: 'Unknown',
  };

  signup(){
    if(this.signupForm.value.password && this.signupForm.value.confirmPassword){
      this.comparePassword(this.signupForm.value.password, this.signupForm.value.confirmPassword);
    }
    if(!this.signupForm.valid || !this.isPasswordMatch){
      this.isSubmitted = true;
      let passwordElement: HTMLButtonElement = document.getElementsByClassName('passwordEye')[0] as HTMLButtonElement;
      passwordElement.style.top = "55%";
      return;
    }
    
    this.signupFormValue = this.signupForm.value as signupViewModel;
    this._authService.signup(this.signupFormValue).subscribe((response: signupResponseViewModel) =>{
      if(response.isSuccess){
        let signupMessage = 'Signup was success';
        this._messageService.add({
          severity: "success",
          summary: "Success",
          life: 3000,
          detail: signupMessage
        })
        setTimeout(() => {
          this._router.navigateByUrl('/auth/login');
        }, 1000);
      } else{
        let signupMessage = 'Email already exist';
        this._messageService.add({
          severity: "success",
          summary: "Success",
          life: 3000,
          detail: signupMessage
        })
      }
    });
  }

  passwordChange(){
    this.password = this.pass ? "password" : "text";
    this.pass = !this.pass;
  }

  confirmPasswordChange(){
    this.confirmPassword = this.confirmPass ? "password" : "text";
    this.confirmPass = !this.confirmPass;
  }

  comparePassword(password:string, confirmPassword:string){
    if(password == confirmPassword){
      this.isPasswordMatch  = true
    } else{
      this.isPasswordMatch = false;
    }
  }


  getCity(country: any){
    if(country)
    if(country.trim()){
      this._placeService.getCities(country).subscribe(response => {
        this.cities = response;
      })
    } else{
      this.cities = []
    }
  }



}
