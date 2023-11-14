import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalService } from 'ngx-bootstrap/modal';
import { Subject, Subscription } from 'rxjs';
import { ChatHeadViewModel } from 'src/app/Interfaces/chat-head-view-model';
import { postViewModel } from 'src/app/Interfaces/postModel';
import { EditUserDetailsViewModel, UserViewModel } from 'src/app/Interfaces/user-view-model';
import { addPost } from 'src/app/Post/add-post/add-post.component';
import { ChatService } from 'src/app/Services/chat.service';
import { PostService } from 'src/app/Services/post.service';
import { UserService } from 'src/app/Services/user.service';

export const messageFromUserProfile = new Subject<{chatHead: ChatHeadViewModel, userId: string}>();

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit, OnDestroy {

  private _userService;
  private _router;
  private _postService;
  private _chatService;

  userDetails!:UserViewModel;
  profilePic: string = '';
  allPosts: postViewModel[] = [];
  addPostSubscription!:Subscription;
  userId: string = '';
  loginUserId: string = '';
  userName: string = '';

  constructor(userService: UserService, 
    chatService: ChatService, 
    router: ActivatedRoute, 
    postService: PostService, 
    private route:Router,
    private modalService: BsModalService
  ) { 
    this._userService = userService;
    this._router = router;
    this._postService = postService;
    this._chatService = chatService;
  }

  ngOnDestroy(): void {
    if(this.addPostSubscription){
      this.addPostSubscription.unsubscribe();
    };
  }

  ngOnInit(): void {
    this.getLoggedInUserDetails();
    this._router.params.subscribe(params => {
      this.userId = params['userId'];
    });
    if(this.userId){
      this.getUserDetails(this.userId);
      this.getUsersPost(this.userId);
    }
    if(!this.addPostSubscription){
      this.addPostSubscription = addPost.subscribe(response => {
        if(response.isPostAdded){
          this.ngOnInit();
        }
      })
    }
  }

  changeTime(createdTime: string): string {
    const currentDate = new Date();
    const creationDate = new Date(createdTime);

    const diffInMilliseconds = currentDate.getTime() - creationDate.getTime();
    const diffInHours = diffInMilliseconds / (1000 * 60 * 60);

    if (diffInHours < 1) {
      const diffInMinutes = diffInMilliseconds / (1000 * 60);
      return `${Math.floor(diffInMinutes)} minutes ago`;
    } else if (diffInHours < 24) {
      return `${Math.floor(diffInHours)} hours ago`;
    } else {
      const diffInDays = diffInHours / 24;
      return `${Math.floor(diffInDays)} days ago`;
    }
  }

  getUserDetails(userId: string){
    this._userService.getUserDetails(userId).subscribe(response => {
      if(response.isSuccess){
        this.userDetails = response.data;
        this.userName = response.data.firstName + ` ${response.data.lastName}`;
        if(this.userDetails.avatar.trim()){
          this.profilePic = this.userDetails.avatar;
        } else{
          if (response.data.gender.toLowerCase() == 'male') {
            this.profilePic = 'assets/male-avatar.avif';
          } else if(response.data.gender.toLowerCase() == 'female') {
            this.profilePic = 'assets/female-avatar.avif';
          } else{
            this.profilePic = 'assets/rather-not-say.avif';
          }
        }
      }
    })
  }

  getUsersPost(userId: string){
    this._postService
    .getUsersAllPosts(userId, 1, 10)
    .subscribe((response) => {
      this.allPosts = response.post;
    });
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

  message(){
    this._chatService.isChatHeadExist(this.loginUserId, this.userDetails.id).subscribe(response => {
      if(response.isSuccess){
        let chatHead: ChatHeadViewModel = response.data;
        chatHead.user = this.userDetails;
        this.route.navigateByUrl(`/chatView/${response.data.chatHeadId}`);
        setTimeout(() => {
          messageFromUserProfile.next({
            chatHead: chatHead,
            userId: this.userDetails.id
          });
        }, 0);
      }
    });
  }

  getProfilePic(post: postViewModel): string{
    if(post.user.avatar.trim()){
      return post.user.avatar;
    } else{
      if(post.user.gender.toLowerCase() == 'male'){
        return "assets/male-avatar.avif";
      } else if(post.user.gender.toLowerCase() == 'female'){
        return "assets/female-avatar.avif"
      } else{
        return "assets/rather-not-say.avif"
      }
    }
  }

  editUser(){
    this.route.navigateByUrl('/editUser');
  }

}
