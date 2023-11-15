import { Component, HostListener, Input, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { postViewModel } from 'src/app/Interfaces/postModel';
import { PostService } from 'src/app/Services/post.service';
import { addPost } from '../add-post/add-post.component';
import { AngularFireStorage } from '@angular/fire/compat/storage';
import { MessageService } from 'primeng/api';

export const updatePost = new Subject<{ post: postViewModel }>();

@Component({
  selector: 'app-post-view',
  templateUrl: './post-view.component.html',
  styleUrls: ['./post-view.component.css'],
  providers: [MessageService]
})
export class PostViewComponent implements OnInit {
  private _postService;
  private _messageService;
  allPosts: postViewModel[] = [];
  homePageNumber: number = 1;
  homePageSize: number = 10;
  userProfilePageNumber: number = 1;
  userProfilePageSize: number = 10;
  @Input() userId: string = '';
  @Input() isHomeComponent: boolean = false;
  isPostDeleteAllowed: boolean = false;
  loginUserId: string = '';
  stopHomeCall: boolean = false;
  stopUserCall: boolean = false;
  updateDateAndTime: string = '';

  constructor(postService: PostService, private fireStorage: AngularFireStorage, messageService: MessageService) {
    this._postService = postService;
    this._messageService = messageService;
  }

  ngOnInit(): void {
    this.getLoggedInUserDetails();
    if (!this.isHomeComponent) {
      this.getUsersPost(this.userId);
      this.isPostDeleteAllowed = true;
    } else {
      this.isPostDeleteAllowed = false;
      this.getUserHomePosts();
    }

    addPost.subscribe((response) => {
      if (response.isPostAdded) {
        this.allPosts.unshift(response.post);
      } else{
        let findIndex = this.allPosts.findIndex(x => x.postId == response.post.postId);
        if(findIndex !== -1){
          this.allPosts[findIndex] = response.post;
        }
      }
    });
  }

  getUserHomePosts() {
    this._postService
      .getAllPosts(this.userId, this.homePageNumber, this.homePageSize)
      .subscribe((response) => {
        if(response.post == null){
          this.allPosts = [];
        } else{
          this.allPosts = response.post;
        }
      });
  }

  getUsersPost(userId: string) {
    this._postService
      .getUsersAllPosts(
        userId,
        this.userProfilePageNumber,
        this.userProfilePageSize
      )
      .subscribe((response) => {
        if(response.post == null){
          this.allPosts = [];
        } else{
          this.allPosts = response.post;
        }
      });
  }

  getProfilePic(post: postViewModel): string {
    if (post.user.avatar.trim()) {
      return post.user.avatar;
    } else {
      if (post.user.gender.toLowerCase() == 'male') {
        return 'assets/male-avatar.avif';
      } else if (post.user.gender.toLowerCase() == 'female') {
        return 'assets/female-avatar.avif';
      } else {
        return 'assets/rather-not-say.avif';
      }
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

  deletePost(post: postViewModel) {
    if(post.fileUrlType != 'Default'){
      let postName = this.extractFilenameFromUrl(post.fileUrl);
      this.deleteFile(`${post.fileUrlType}/${postName}`).subscribe(
        () => {
          this.delPost(post);
        },
        (error) => console.error('Error deleting file:', error)
      );
    } else{
      this.delPost(post);
    }
  }

  isPostAllowedForDelete(parentId: string): boolean {
    if (this.loginUserId == parentId) {
      return true;
    } else {
      return false;
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

  getMoreHomePosts() {
    this.homePageNumber++;
    this._postService
      .getAllPosts(this.loginUserId, this.homePageNumber, this.homePageSize)
      .subscribe((response) => {
        if (response.post) {
          response.post.forEach((element: postViewModel) => {
            this.allPosts.push(element);
          });
        }
        if(response.post?.length < this.homePageSize){
          this.stopHomeCall = true;
        }
      });
  }

  getMoreUserProfilePosts() {
    this.userProfilePageNumber++;
    this._postService
      .getUsersAllPosts(
        this.loginUserId,
        this.userProfilePageNumber,
        this.userProfilePageSize
      )
      .subscribe((response) => {
        if (response.post) {
          response.post.forEach((element: postViewModel) => {
            this.allPosts.push(element);
          });
        }
      });
  }

  updatePostById(post: postViewModel) {
    updatePost.next({
      post: post,
    });
  }

  likeUnlikePost(postId: string, isPostLiked: boolean) {
    this._postService
      .likeUnlikePost(postId, this.loginUserId)
      .subscribe((response) => {
        if(response.isSuccess){
          if(isPostLiked){
            let postIndex = this.allPosts.findIndex(x => x.postId == postId);
            if (postIndex !== -1) {
              let post = this.allPosts[postIndex];
              post.isLikedByCurrentUser = false;
              post.totalLikes--;
            }
          } else{
            let postIndex = this.allPosts.findIndex(x => x.postId == postId);
            if (postIndex !== -1) {
              let post = this.allPosts[postIndex];
              post.isLikedByCurrentUser = true;
              post.totalLikes++;
            }
          }
        }
      });
  }

  @HostListener('window:scroll', ['$event'])
  onScroll() {
    const scrollPosition = window.pageYOffset;
    const windowHeight = window.innerHeight;
    const bodyHeight = document.body.scrollHeight;

    const scrollPercent = (scrollPosition / (bodyHeight - windowHeight)) * 100;

    if (scrollPercent > 90 && (!this.stopHomeCall || !this.stopUserCall)) {
      if (!this.isHomeComponent) {
        this.getMoreUserProfilePosts();
      } else{
        this.getMoreHomePosts();
      }
    } else {
      return;
    }
  }

  deleteFile(filePath: string): Observable<void> {
    return new Observable((observer) => {
      const fileRef = this.fireStorage.ref(filePath);
      fileRef.delete().subscribe(
        () => {
          observer.next();
          observer.complete();
        },
        (error) => observer.error(error)
      );
    });
  }

  extractFilenameFromUrl(url: string): string {
    const decodedUrl = decodeURIComponent(url);
    const filenameWithDirectory = decodedUrl.split("/o/")[1].split("?")[0];
    const filename = filenameWithDirectory.split("/").pop();
    return filename || '';
  }

  delPost(post: postViewModel){
    this._postService.deletePostById(post.postId).subscribe((response) => {
      if (response.isSuccess) {
        let postIndex = this.allPosts.findIndex(
          (x) => x.postId == response.post.postId
        );
        if (postIndex !== -1) {
          this.allPosts.splice(postIndex, 1);
          let postDeleted = 'Post is deleted';
          this._messageService.add({
            severity: "error",
            summary: "Delete",
            life: 3000,
            detail: postDeleted
          });
        }
      } else {
        console.log('deletePost: ', response.errorMessage);
      }
    });
  }

  getPostEditTime(time: string) {
    if (time == null) {
      this.updateDateAndTime = '';
    } else {
      const postDate = new Date(time);
      const currentDate = new Date();
  
      const timeDifferenceInMillis = currentDate.getTime() - postDate.getTime();
  
      if (timeDifferenceInMillis < 3600000) {
        const minutesDifference = Math.floor(timeDifferenceInMillis / (1000 * 60));
        this.updateDateAndTime = `${minutesDifference} ${minutesDifference === 1 ? 'minute' : 'minutes'} ago`;
      } else if (
        postDate.getDate() === currentDate.getDate() &&
        postDate.getMonth() === currentDate.getMonth() &&
        postDate.getFullYear() === currentDate.getFullYear()
      ) {
        const hoursDifference = Math.floor(timeDifferenceInMillis / (1000 * 60 * 60));
        this.updateDateAndTime = `${hoursDifference} ${hoursDifference === 1 ? 'hour' : 'hours'} ago`;
      } else {
        const options: Intl.DateTimeFormatOptions = {
          day: 'numeric',
          month: 'short',
          year: 'numeric',
        };
        this.updateDateAndTime = postDate.toLocaleDateString('en-US', options);
      }
    }
  }
  

}
