import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { addPostViewModel, postViewModel } from '../Interfaces/postModel';
import { PostService } from '../Services/post.service';
import { UserService } from '../Services/user.service';
import { FileUrlType } from '../enum/file-url-type';
// import { Picker } from 'emoji-mart';
import { MessageService } from 'primeng/api';
import { addPost } from '../Post/add-post/add-post.component';
import { Router } from '@angular/router';

declare var $: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [MessageService]
})
export class HomeComponent implements OnInit, AfterViewInit {
  private _postService;
  private _userService;
  private _messageService;
  private _router;
  file!: File;
  isSubmitted: boolean = false;
  addPostValue!: addPostViewModel;
  loginUserId: string = '';
  userName: string = '';
  avatarSrc: string = '';
  firstName: string = '';
  openPostButton: boolean = false;
  showEmojiPicker = false;
  allPosts: postViewModel[] = [];
  pageNumber: number = 1;
  pageSize: number = 10;
  isAllPostFinished: boolean = false;
  previousScrollTop: number = 0;

  @ViewChild('inputField') inputField!: ElementRef;
  @ViewChild('emojiContainer') emojiContainer!: ElementRef;

  constructor(postService: PostService, 
    userService: UserService, messageService:MessageService,
    private router: Router
  ) {
    this._postService = postService;
    this._userService = userService;
    this._messageService = messageService;
    this._router = router;
  }
  ngAfterViewInit(): void {
    $('#exampleModalCenter').on('shown.bs.modal', () => {
      this.inputField.nativeElement.focus();
      document.body.style.overflow = 'hidden';
    });
    $('#exampleModalCenter').on('hidden.bs.modal', function () {
      document.body.style.overflow = 'auto';
    });
  }

  ngOnInit(): void {
    this.getLoggedInUserDetails();
    this.getUserDetails();
  }

  getUserDetails(){
    this._userService.getUserDetails(this.loginUserId).subscribe((response) => {
      this.firstName = response.data.firstName;
      this.userName = response.data.firstName + ` ${response.data.lastName}`;
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
      }
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

  goToUserProfile(){
   this._router.navigateByUrl('/user/' + this.loginUserId); 
  }
  
}
