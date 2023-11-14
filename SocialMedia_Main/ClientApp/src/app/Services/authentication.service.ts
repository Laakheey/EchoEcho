import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { signupResponseViewModel, signupViewModel } from '../Interfaces/signupViewModel';
import { Observable } from 'rxjs';
import { loginViewModel } from '../Interfaces/loginViewModel';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private httpClient:HttpClient) { }
  rootUrl: string= 'https://localhost:7202/api/'
  signup(signupModel: signupViewModel) : Observable <any>{
    return this.httpClient.post(this.rootUrl + "Authentication/register", signupModel);
  }

  login(loginViewModel: loginViewModel) : Observable <any>{
    return this.httpClient.post(this.rootUrl + "Authentication/login", loginViewModel);
  }

  logout() : Observable <any>{
    return this.httpClient.get(this.rootUrl + "Authentication/logout");
  }

  refreshToken(userId: string) : Observable<any>{
    return this.httpClient.get(`${this.rootUrl}Authentication/refreshToken?userId=${userId}`);
  }

}
