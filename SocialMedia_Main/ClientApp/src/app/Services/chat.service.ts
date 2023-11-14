import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AddMessageViewModel } from '../Interfaces/chat-head-view-model';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  rootUrl: string = 'https://localhost:7202/api/Chat/';
  token:string | null ='';
  constructor(private httpClient: HttpClient) { 
    this.token = localStorage.getItem('token');
  }
  getUserChatHeads(userId:string, pageNumber:number, pageSize:number, searchString:string) : Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this.httpClient.get(`${this.rootUrl}getUserChatHeads?userId=${userId}&pageNumber=${pageNumber}&pageSize=${pageSize}&searchString=${searchString}`, {
      headers: httpHeader
    }
    );
  }

  sendMessage(addMessageViewModel: AddMessageViewModel) : Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this.httpClient.post(this.rootUrl + "sendMessage",addMessageViewModel, {
      headers: httpHeader
    }
    );
  }

  getUserChats(receiverId:string, pageNumber:number, pageSize:number) : Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    });
    return this.httpClient.get(`${this.rootUrl}getUserMessage?receiverId=${receiverId}&pageNumber=${pageNumber}&pageSize=${pageSize}`, {
      headers: httpHeader
    }
    );
  }

  getUserChatHeadById(chatHeadId: string, senderId: string) : Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    });
    return this.httpClient.get(`${this.rootUrl}getUserChatHeadById?chatHeadId=${chatHeadId}&senderId=${senderId}`, {
      headers: httpHeader
    }
    );
  }

  getUserChatsById(chatHeadId: string) : Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    });
    return this.httpClient.get(`${this.rootUrl}getUserChatsById?chatHeadId=${chatHeadId}`, {
      headers: httpHeader
    }
    );
  }

  isChatHeadExist(currentSenderId: string, currentReceiverId: string) : Observable<any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this.httpClient.get(`${this.rootUrl}isChatHeadExist?currentSenderId=${currentSenderId}&currentReceiverId=${currentReceiverId}`, {
      headers : httpHeader
    });
  }

}
