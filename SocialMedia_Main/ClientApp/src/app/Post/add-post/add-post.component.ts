import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Message, MessageService } from 'primeng/api';
import { DomSanitizer } from '@angular/platform-browser';
import { addPostViewModel, postViewModel, updatePostViewModel } from 'src/app/Interfaces/postModel';
import { PostService } from 'src/app/Services/post.service';
import { UserService } from 'src/app/Services/user.service';
import { FileUrlType } from 'src/app/enum/file-url-type';
import { Observable, Subject } from 'rxjs';
import { updatePost } from '../post-view/post-view.component';
import { finalize } from 'rxjs/operators';

declare var $: any;
export const addPost = new Subject<{isPostAdded: boolean, post: postViewModel}>();

import { AngularFireStorage } from '@angular/fire/compat/storage';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-add-post',
  templateUrl: './add-post.component.html',
  styleUrls: ['./add-post.component.css'],
  providers: [MessageService]
})
export class AddPostComponent implements OnInit, AfterViewInit {
  private _postService;
  private _userService;
  private _messageService;
  file!: File;
  isSubmitted: boolean = false;
  addPostValue!: addPostViewModel;
  loginUserId: string = '';
  userName: string = '';
  avatarSrc: string = '';
  firstName: string = '';
  openPostButton: boolean = false;
  showEmojiPicker = false;
  thumbnailUrl: string = "";
  isImage: boolean = false;
  isEditPost: boolean = false;
  fileToUpload: File | null = null;
  fileUrl: string = '';
  fileUrlType: string = '';
  fileChange: boolean = false;
  progressToast!: Message;
  uploadProgress: number = 0;
  isUploadStart:boolean = false;

  @ViewChild('inputField') inputField!: ElementRef;
  @ViewChild('emojiContainer') emojiContainer!: ElementRef;
  modalRef?: BsModalRef;
  @ViewChild('template') modal!: TemplateRef<any>;

  constructor(postService: PostService, 
    userService: UserService, messageService: MessageService,
    private sanitizer: DomSanitizer,
    private fireStorage: AngularFireStorage,
    private modalService: BsModalService,
    public options: ModalOptions,
  ) {
    this._postService = postService;
    this._userService = userService;
    this._messageService = messageService;
  }
  ngAfterViewInit(): void {
  }

  ngOnInit(): void {
    this.getLoggedInUserDetails();
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

    updatePost.subscribe(response => {
      this.openModal();
      this.addPostForm.patchValue({
        postId: response.post.postId,
        description: response.post.description || '',
        fileUrl: response.post.fileUrl,
        fileUrlType: response.post.fileUrlType || FileUrlType.default.toString()
      });
      this.isEditPost = true;
      this.thumbnailUrl = response.post.fileUrl;
      this.fileUrlType = response.post.fileUrlType
      if(response.post.fileUrlType == 'Image'){
        this.isImage = true;
      } else{
        this.isImage = false;
      }
    });

    window.addEventListener('beforeunload', (e)=>{
      if(this.isUploadStart){
        e.preventDefault();
        e.returnValue = '';
        const confirmationMessage = 'Are you sure you want to leave? Your changes may not be saved.';
        return confirmationMessage;
      }
      return
    });
  }

  addPostForm = new FormGroup({
    postId: new FormControl(''),
    description: new FormControl(''),
    fileUrl: new FormControl(''),
    fileUrlType: new FormControl(''),
  });

  getDescription() {
    this.addPostForm.get('description')?.valueChanges.subscribe((value) => {
      if (!value?.trim()) {
        if(!this.thumbnailUrl?.trim()){
          this.openPostButton = false;
        } else{
          this.openPostButton = true;
        }
      } else {
        this.openPostButton = true;
      }
    });
  }

  onFileChange(event: any) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      this.handleFiles(input.files);
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

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    (<HTMLElement>event.target).classList.add('dragover');
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    (<HTMLElement>event.target).classList.remove('dragover');
    const files = event.dataTransfer?.files;
    if (files && files.length) {
      const file = files[0];
      this.handleFiles(files);
    }
  }

  removeFile(){
    this.thumbnailUrl = "";
    if(this.addPostForm.get('description')?.value){
      this.openPostButton = true;
    } else{
      this.openPostButton = false;
    }
    this.fileUrl = '';
    this.fileUrlType = '';
    this.fileChange = true;
  }

  handleFiles(files: FileList) {
    if(files.length >= 2){
      let fileNotAllowed = 'Multiple files not allowed';
        this._messageService.add({
          severity: "success",
          summary: "Success",
          life: 3000,
          detail: fileNotAllowed
        });
      return;
    }
    const file = files[0];
    const fileType = file.type.split('/')[0];
    this.isImage = fileType === 'image';
    if(!(fileType == 'image' || fileType == 'video')){
      let fileAdded = 'Only image or video can be added.';
        this._messageService.add({
          severity: "success",
          summary: "Success",
          life: 3000,
          detail: fileAdded
        });
      return;
    } else{
      this.thumbnailUrl = this.sanitizer.bypassSecurityTrustResourceUrl(URL.createObjectURL(file)) as string;
      this.openPostButton = true;
      this.fileToUpload = file;
      this.fileChange = true;
      if(fileType == 'image'){
        this.fileUrlType = 'Image';
      } else{
        this.fileUrlType = 'Video';
      }
    }
  }

  showModal(){
    setTimeout(() => {
      let fileUrlType = this.addPostForm.get('fileUrlType')?.value;
      if(fileUrlType){
        this.openPostButton = true;
      } else{
        this.openPostButton = false;
      }
    }, 0);
    $('#addPostModal').modal('show');
  }

  uploadFile(): Observable<string> {
    return new Observable((observer) => {
      if (!this.fileToUpload) {
        observer.next('');
        observer.complete();
        return;
      }
      let fileName = this.generateRandomString() + `_${this.fileUrlType}_firebase`;
      const filePath = `${this.fileUrlType}/${fileName}`;
      const fileRef = this.fireStorage.ref(filePath);
      const uploadTask = this.fireStorage.upload(filePath, this.fileToUpload);
  
      uploadTask.percentageChanges().subscribe((progress) => {
        console.log(`Upload is ${progress}% done`);
        this.isUploadStart = true;
        this.uploadProgress = Math.floor(progress as number);
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

  addPost() {
    if (!this.addPostForm.valid) {
      this.isSubmitted = true;
      return;
    }
    this.closeModal();
    if (!this.isEditPost) {
      this.addPostFunction();
    } else {
      this.updatePost();
    }
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

  openModal(){
    this.modalRef = this.modalService.show(this.modal, {
      ignoreBackdropClick: true,
      keyboard: false,
    });
    setTimeout(() => {
      let fileUrlType = this.addPostForm.get('fileUrlType')?.value;
      if(fileUrlType){
        this.openPostButton = true;
      } else{
        this.openPostButton = false;
      }
    }, 0);

  }

  closeModal(isReset?: boolean){
    this.modalService.hide();
    if(isReset){
      this.resetModalValue();
    }
  }

  updatePost(){
    debugger;
    if(this.fileChange && this.isEditPost){
      this.uploadFile().subscribe((downloadURL) => {
        this.fileUrl = downloadURL;
        let updateData: updatePostViewModel = {
          postId: this.addPostForm.get('postId')?.value || '',
          description: this.addPostForm.get('description')?.value || '',
          fileUrl: this.fileUrl || '',
          fileUrlType: this.fileUrlType || 'Default'
        };
        this._postService.updatePost(updateData).subscribe(response => {
          if(response.isSuccess){
            let postUpdated = "Post updated"
            this._messageService.add({
              severity: "success",
              summary: "Success",
              life: 3000,
              detail: postUpdated
            });
            this.resetModalValue();
            addPost.next({isPostAdded : false, post: response.post});
          }
        });
      });
      return;
    }
    let updateData: updatePostViewModel = {
      postId: this.addPostForm.get('postId')?.value || '',
      description: this.addPostForm.get('description')?.value || '',
      fileUrl: this.thumbnailUrl || null,
      fileUrlType: this.fileUrlType || 'Default'
    };
    this._postService.updatePost(updateData).subscribe(response => {
      if(response.isSuccess){
        let postUpdated = "Post updated"
        this._messageService.add({
          severity: "success",
          summary: "Success",
          life: 3000,
          detail: postUpdated
        });
        this.resetModalValue();
        addPost.next({isPostAdded : false, post: response.post});
      }
    });
  }

  addPostFunction(){
    this.uploadFile().subscribe((downloadURL) => {
      this.fileUrl = downloadURL;
      let postData: addPostViewModel = {
        postId: this.addPostForm.get('postId')?.value || '',
        description: this.addPostForm.get('description')?.value || '',
        fileUrl: this.fileUrl || null,
        fileUrlType: this.fileUrlType || 'Default'
      };
      this._postService.addNewPost(postData).subscribe(response => {
        if(response.isSuccess){
          let postAdded = 'New post created';
          this._messageService.add({
            severity: "success",
            summary: "Success",
            life: 3000,
            detail: postAdded
          });
          this.resetModalValue();
          addPost.next({isPostAdded : true, post: response.post});
        }
      });
    });
  }

  resetModalValue(){
    this.addPostForm.reset();
    this.isEditPost = false;
    this.isUploadStart = false;
    this.thumbnailUrl = '';
    this.fileToUpload = null;
    this.fileUrlType = "Default";
    this.fileChange = false;
  }
}
