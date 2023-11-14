import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EditUserDetailsViewModel } from '../Interfaces/user-view-model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private _httpClient;
  token: string | null = '';
  constructor(httpClient: HttpClient) { 
    this._httpClient = httpClient;
    this.token = localStorage.getItem('token');
  }

  private getToken(): string | null {
    return localStorage.getItem('token');
  }

  rootUrl: string = "https://localhost:7202/api/User/";

  getUserDetails(userId: string) : Observable<any> {
    this.token = this.getToken();
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this._httpClient.get(`${this.rootUrl}getUserDetails?userId=${userId}`, {
      headers: httpHeader
    });
  }

  getSearchUser(searchString: string) : Observable<any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this._httpClient.get(`${this.rootUrl}getSearchUser?searchString=${searchString}`, {
      headers: httpHeader
    });
  }

  editUserDetails(editUserDetails: EditUserDetailsViewModel): Observable<any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this._httpClient.post(`${this.rootUrl}editUserDetails`,editUserDetails, {
      headers: httpHeader
    });
  }



}
