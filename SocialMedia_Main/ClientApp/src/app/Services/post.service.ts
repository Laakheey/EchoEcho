import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { addPostViewModel, updatePostViewModel } from '../Interfaces/postModel';

@Injectable({
  providedIn: 'root'
})
export class PostService {
  token: string | null = '';
  constructor(private httpClient: HttpClient) { 
    this.token = localStorage.getItem('token');
  }
  rootUrl: string = 'https://localhost:7202/api/Post/';
  url = "https://localhost:7202/api/Post/likeUnlikePost?postId=3de4eab0-9887-445b-baa4-c64801f21600&userId=310c8d6c-799a-4567-a183-d2e22575f5e3";
  addNewPost(addPostViewModel: addPostViewModel):Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this.httpClient.post(this.rootUrl + "addNewPost", addPostViewModel, {
      headers: httpHeader
    });
  }

  updatePost(addPostViewModel: updatePostViewModel):Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this.httpClient.post(this.rootUrl + "updatePost", addPostViewModel, {
      headers: httpHeader
    });
  }

  getAllPosts(parentId: string, pageNumber: number, pageSize: number): Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this.httpClient.get(`${this.rootUrl}getAllPost?parentId=${parentId}&pageNumber=${pageNumber}&pageSize=${pageSize}`, {
      headers: httpHeader
    });
  }

  getUsersAllPosts(parentId: string, pageNumber: number, pageSize: number): Observable <any>{
    const httpHeader = new HttpHeaders({
      'Authorization': 'Bearer ' + this.token
    })
    return this.httpClient.get(`${this.rootUrl}getUsersAllPost?parentId=${parentId}&pageNumber=${pageNumber}&pageSize=${pageSize}`, {
      headers: httpHeader
    });
  }

  deletePostById(postId: string): Observable<any>{
    const httpHeader = new HttpHeaders({
      'Authorization' : 'Bearer ' + this.token
    })
    return this.httpClient.delete(`${this.rootUrl}deletePostById?postId=${postId}`, {
      headers: httpHeader
    });
  }

  likeUnlikePost(postId: string, userId: string): Observable<any>{
    const httpHeader = new HttpHeaders({
      'Authorization' : 'Bearer ' + this.token
    });
    return this.httpClient.post(`${this.rootUrl}likeUnlikePost?postId=${postId}&userId=${userId}`,{}, {
      headers: httpHeader
    });
  }

  // likeUnlikePost(postId: string, userId: string):Observable <any>{
  //   const httpHeader = new HttpHeaders({
  //     'Authorization': 'Bearer ' + this.token
  //   })
  //   return this.httpClient.post(`${this.rootUrl}?postId=${postId}&userId=${userId}`, {
  //     headers: httpHeader
  //   });
  // }

}
