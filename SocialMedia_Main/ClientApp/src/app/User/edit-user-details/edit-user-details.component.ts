import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AngularFireStorage } from '@angular/fire/compat/storage';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Observable } from 'rxjs';
import { genderType } from 'src/app/Interfaces/signupViewModel';
import { EditUserDetailsViewModel } from 'src/app/Interfaces/user-view-model';
import { UserService } from 'src/app/Services/user.service';
import { finalize } from 'rxjs/operators'
import { Router } from '@angular/router';
import { PlaceService } from 'src/app/Services/place.service';
import { CountryViewModel } from 'src/app/Interfaces/country-view-model';

@Component({
  selector: 'app-edit-user-details',
  templateUrl: './edit-user-details.component.html',
  styleUrls: ['./edit-user-details.component.css']
})
export class EditUserDetailsComponent implements OnInit {
 
  private _userService;
  private _placeService;
  isSubmitted: boolean = false;
  avatarSrc: string = '';
  loginUserId: string = '';
  userDetails!: EditUserDetailsViewModel;
  isImageChange: boolean = false;
  avatarToUpload!: File;
  avatarExistUrl: string = '';
  countries:CountryViewModel[] = [];
  cities:string[] = [];
  showRemoveButton: boolean = false;

  @ViewChild('fileInput') fileInput!: ElementRef;
  
  constructor( 
    private fb: FormBuilder, 
    userService: UserService,
    private sanitizer: DomSanitizer,
    private fireStorage: AngularFireStorage,
    private router: Router,
    placeService: PlaceService
  ){
    this._userService = userService;
    this._placeService = placeService;
  }

  ngOnInit(): void {
    this.getLoggedInUserDetails()
    this._userService.getUserDetails(this.loginUserId).subscribe((response) => {
      this.userDetails = response.data;
      let formatDate: string = new Date().toISOString().substring(0, 10).toString();
      if(this.userDetails.dateOfBirth != null){
        let date = this.userDetails.dateOfBirth.split('T')[0];
        formatDate = new Date(date).toISOString().substring(0, 10);
      }
      this.editUserForm.patchValue({
        firstName: this.userDetails.firstName,
        lastName: this.userDetails.lastName,
        dateOfBirth: formatDate,
        phoneNumber: this.userDetails.phoneNumber,
        gender: this.userDetails.gender,
        country: this.userDetails.country,
        city: this.userDetails.city
      });
      if (!response.data.avatar) {
        if (response.data.gender.toLowerCase() == 'male') {
          this.avatarSrc = 'assets/male-avatar.avif';
        } else if(response.data.gender.toLowerCase() == 'female') {
          this.avatarSrc = 'assets/female-avatar.avif';
        } else{
          this.avatarSrc = 'assets/rather-not-say.avif';
        }
      } else {
        this.avatarSrc = response.data.avatar;
        this.avatarExistUrl = this.avatarSrc;
        this.showRemoveButton = true;
      }
      this.getCity(this.userDetails.country);
    });

    this._placeService.getCountries().subscribe(response => {
      this.countries = response;
    });
  }

  editUserForm: FormGroup = this.fb.group({
    firstName : new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(15)]),
    lastName : new FormControl('', []),
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

  editUserDetails(){
    this.isSubmitted = true;
    if(!this.editUserForm.valid){
      return;
    }
    if(!this.isImageChange){
      let userDetails = this.editUserForm.value as EditUserDetailsViewModel
      if(this.avatarExistUrl.trim()){
        userDetails.avatar = this.avatarExistUrl;
      } else{
        userDetails.avatar = "";
      }
      this._userService.editUserDetails(userDetails).subscribe(response => {
        this.router.navigateByUrl('/');
      })
    } else{
      this.uploadFile().subscribe((downloadURL) => {
        let userDetails = this.editUserForm.value as EditUserDetailsViewModel
        userDetails.avatar = downloadURL;
        this._userService.editUserDetails(userDetails).subscribe(response => {
          this.router.navigateByUrl('/');
        });
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
    }
  }

  openFileInput() {
    this.fileInput.nativeElement.click();
  }
  
  handleFileInput(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files && inputElement.files[0]) {
      const selectedFile = inputElement.files[0];
      this.avatarToUpload = selectedFile;
      this.avatarSrc = this.sanitizer.bypassSecurityTrustResourceUrl(URL.createObjectURL(selectedFile)) as string;
      this.isImageChange = true;
      this.showRemoveButton = true;
    }
  }

  uploadFile(): Observable<string> {
    return new Observable((observer) => {
      if (!this.isImageChange) {
        observer.next('');
        observer.complete();
        return;
      }
  
      let fileName = this.generateRandomString() + `_avatar_firebase`;
      const filePath = `avatar/${fileName}`;
      const fileRef = this.fireStorage.ref(filePath);
      const uploadTask = this.fireStorage.upload(filePath, this.avatarToUpload);
  
      uploadTask.percentageChanges().subscribe((progress) => {
        console.log(`Upload is ${progress}% done`);
      });
  
      uploadTask.snapshotChanges().pipe(
        finalize(() => {
          fileRef.getDownloadURL().subscribe((downloadURL) => {
            observer.next(downloadURL);
            observer.complete();
          });
        })
      ).subscribe();
    });
  }
  
  generateRandomString(length = 6): string {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
      const randomIndex = Math.floor(Math.random() * characters.length);
      result += characters.charAt(randomIndex);
    }
    return result;
  }

  removeImage(){
    this.avatarExistUrl = '';
    if (this.userDetails.gender.toLowerCase() == 'male') {
      this.avatarSrc = 'assets/male-avatar.avif';
    } else if(this.userDetails.gender.toLowerCase() == 'female') {
      this.avatarSrc = 'assets/female-avatar.avif';
    } else{
      this.avatarSrc = 'assets/rather-not-say.avif';
    }
    this.fileInput.nativeElement.value = '';
    this.showRemoveButton = false;
  }

  getCity(country: string){
    if(country.trim()){
      this._placeService.getCities(country).subscribe(response => {
        this.cities = response;
      })
    } else{
      this.cities = []
    }
  }

}
