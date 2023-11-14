import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PlaceService {

  private _httpClient;
  rootUrl: string = 'https://localhost:7202/api/Countries/';
  constructor(httpClient: HttpClient) { 
    this._httpClient = httpClient;
  }

  getCountries() : Observable<any>{
    return this._httpClient.get(this.rootUrl + 'getCountries');
  }

  getCities(countryName: string) : Observable<any>{
    return this._httpClient.post(this.rootUrl + `getCities?countryName=${countryName}`,{});
  }

}
