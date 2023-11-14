import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  AddMessageViewModel,
  ChatHeadViewModel,
  MessageResponse,
} from 'src/app/Interfaces/chat-head-view-model';
import { ChatService } from 'src/app/Services/chat.service';
import Pusher from 'pusher-js';
import { Subscription } from 'rxjs';
import { messageFromUserProfile } from 'src/app/User/user-profile/user-profile.component';
import { UserViewModel } from 'src/app/Interfaces/user-view-model';

// var EmojiConvertor = require('emoji-js');

@Component({
  selector: 'app-chatview',
  templateUrl: './chatview.component.html',
  styleUrls: ['./chatview.component.css'],
})
export class ChatviewComponent implements OnInit, AfterViewInit {
  private _chatServices;

  loginUserId: string = '';
  pageNumber: number = 1;
  pageSize: number = 10;
  searchString: string = '';
  isChatHeadOpen: boolean = false;
  receivers: ChatHeadViewModel[] = [];
  selectedUserName: string = '';
  selectedUser!: ChatHeadViewModel;
  addMessageViewModel!: AddMessageViewModel;
  messageResponse: MessageResponse[] = [];
  message: string = '';
  previousScrollTop: number = 0;
  allMessagesLoaded: boolean = false;
  chatHeadId: string | null = '';
  previousSelectedElement: any = null;
  lastMessage: string = '';
  receiverId: string | null = '';
  pusher: any;
  channel: any;
  newMessage!: MessageResponse;
  messageFromProfileSubscription!: Subscription;
  profilePic: string = '';

  @ViewChild('messagesElem') messagesElem!: ElementRef;
  @ViewChild('messageInputBox') messageInputBox!: ElementRef;

  constructor(
    chatService: ChatService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this._chatServices = chatService;
    this.addMessageViewModel = {
      chatId: '',
      senderId: '',
      receiverId: '',
      chatHeadId: '',
      message: '',
    };

    this.newMessage = {
      chatId: '12345',
      senderId: 's1',
      senderUser: {},
      receiverId: 'r1',
      receiverUser: {},
      message: 'Hello, world!',
      sentTime: '',
      isMessageRead: false,
      chatHead: this.selectedUser,
      chatHeadId: 'ch1',
    };
  }
  ngAfterViewInit(): void {
    setTimeout(() => {
      this.scrollToBottom();
    }, 100);
  }

  ngOnInit(): void {
    this.getLoggedInUserDetails();
    this.chatHeadId = this.route.snapshot.paramMap.get('chatHeadId');
    this._chatServices
      .getUserChatHeads(
        this.loginUserId,
        this.pageNumber,
        this.pageSize,
        this.searchString
      )
      .subscribe((response) => {
        if (response.isSuccess && response.data.length != 0) {
          response.data.forEach((element: ChatHeadViewModel) => {
            this.receivers.push(element);
          });
          this.lastMessage = this.receivers[0].lastMessage;
          if (this.chatHeadId) {
            let receiver = this.receivers.find(
              (x) => x.chatHeadId == this.chatHeadId
            );
            if (receiver) {
              this.openUserChat(receiver);
            }
          }
        }
      });

    let pusher = new Pusher('014bb49542364098f9cb', {
      cluster: 'ap2',
    });

    let channel = pusher.subscribe('chat');
    channel.bind('sendMessage', (data: any) => {
      let incomingMessage = {
        chatHead: data.Data.ChatHead,
        chatHeadId: data.Data.ChatHeadId,
        chatId: data.Data.ChatId,
        isMessageRead: data.Data.IsMessageRead,
        message: data.Data.Message,
        receiverId: data.Data.ReceiverId,
        receiverUser: data.Data.ReceiverUser,
        senderId: data.Data.SenderId,
        senderUser: data.Data.SenderUser,
        sentTime: data.Data.SentTime,
      };
      if (
        !(
          incomingMessage.receiverId == this.loginUserId ||
          incomingMessage.senderId == this.loginUserId
        )
      ) {
        return;
      }
      if (incomingMessage.chatHeadId == this.chatHeadId) 
      {
        this.messageResponse.push(incomingMessage);
      } else if(this.chatHeadId == '00000000-0000-0000-0000-000000000000'){
        this.messageResponse.push(incomingMessage);
        this.chatHeadId = incomingMessage.chatHeadId;
        this.selectedUser.chatHeadId = incomingMessage.chatHeadId;
      }
      let chatHeadPlace = this.receivers.find(
        (x) => x.chatHeadId == incomingMessage.chatHeadId
      );

      if (chatHeadPlace) {
        chatHeadPlace.lastMessage = incomingMessage.message;

      } else{
        this._chatServices.getUserChatHeadById(incomingMessage.chatHeadId, incomingMessage.senderId)
        .subscribe(response => {
          this.receivers.unshift(response.data);
        });
        // this._chatServices.getUserChatHeads(this.loginUserId, 1, 10, `${incomingMessage.senderUser.Id } ${incomingMessage.senderUser.LastName}`).subscribe(response => {
        //   this.receivers.unshift(response.data[0]);
        // })
      }

      this.cdr.detectChanges();
      setTimeout(() => {
        this.scrollToBottom();
      }, 0);
      this.lastMessage = data.Data.ChatHead.LastMessage;
    });

    if(!this.messageFromProfileSubscription){
      messageFromUserProfile.subscribe(response => {
        if(response.chatHead.chatHeadId == '00000000-0000-0000-0000-000000000000'){
          this.receivers.unshift(response.chatHead);
        }
        this.openUserChat(response.chatHead);
        this.cdr.detectChanges();
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

  searchUser(searchString: string) {
    this.searchString = searchString;
    this._chatServices
      .getUserChatHeads(
        this.loginUserId,
        this.pageNumber,
        this.pageSize,
        this.searchString
      )
      .subscribe((response) => {
        if (response.isSuccess) {
          this.receivers = response.data;
        }
      });
  }

  openUserChat(user: ChatHeadViewModel) {
    if (this.selectedUser == user) {
      return;
    } else{
      this.pageNumber = 1;
      this.allMessagesLoaded = false;
    }
    this.chatHeadId = user.chatHeadId;
    this.isChatHeadOpen = true;
    this.selectedUserName =
      user.receiverFirstName + ' ' + user.receiverLastName;
    this.selectedUser = user;
    
    if (this.previousSelectedElement) {
      this.previousSelectedElement.style.backgroundColor = '';
    }
    setTimeout(() => {
      let x = document.getElementById(`${user.receiverId}`);
      if (x) {
        x.style.backgroundColor = 'gray';
        this.previousSelectedElement = x;
      }
    }, 0);

    // let userId: string = '';
    // if(user.receiverId == this.loginUserId){
    //   userId = user.user.id;
    // } else{
    //   userId = user.receiverId;
    // }

    this._chatServices
      .getUserChats(user.user.id, this.pageNumber, this.pageSize)
      .subscribe((response) => {
        if (response.isSuccess) {
          this.messageResponse = response.data;
          this.messageResponse.reverse();
          if (response.data.length < this.pageSize) {
            this.allMessagesLoaded = true;
          };
        };
      });
    setTimeout(() => {
      this.scrollToBottom();
    }, 100);
    this.router.navigate(['/chatView', user.chatHeadId]);
  }

  sendMessage(message: string) {
    this.message = message.trim();
    if (!this.message) {
      return;
    } else {
      console.log('message: ', message);
    }
    this.messageInputBox.nativeElement.value = '';
    if(this.selectedUser.receiverId == this.loginUserId){
      this.addMessageViewModel.receiverId = this.selectedUser.user.id
    } else{
      this.addMessageViewModel.receiverId = this.selectedUser.receiverId;
    }
    this.addMessageViewModel.senderId = this.loginUserId;
    this.addMessageViewModel.message = this.message;
    if(this.messageResponse.length >= 1){
      this.addMessageViewModel.chatHeadId = this.messageResponse[0].chatHeadId;
      this.addMessageViewModel.chatId = this.messageResponse[0].chatId;
    } else{
      if(this.chatHeadId){
        this.addMessageViewModel.chatHeadId = this.chatHeadId;
        this.addMessageViewModel.chatId = this.chatHeadId;
      }
    }

    this._chatServices
      .sendMessage(this.addMessageViewModel)
      .subscribe((response) => {
        if (response.isSuccess) {
          console.log('message sent');
        }
      });
  }

  scrollToBottom(): void {
    if (this.messagesElem && this.messagesElem.nativeElement) {
      const messagesContainer = this.messagesElem.nativeElement;
      messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
  }

  scrollCalled(event: Event) {
    const element = event.target as HTMLElement;
    if (
      element.scrollTop < this.previousScrollTop &&
      element.scrollTop <= 10 &&
      !this.allMessagesLoaded
    ) {
      this.loadMoreChats();
    }
    this.previousScrollTop = element.scrollTop;
  }

  loadMoreChats() {
    this.pageNumber++;
    this._chatServices
      .getUserChats(
        this.selectedUser.receiverId,
        this.pageNumber,
        this.pageSize
      )
      .subscribe((response) => {
        if (response.isSuccess) {
          if (response.data.length < this.pageSize) {
            this.allMessagesLoaded = true;
          }
          response.data.forEach((element: MessageResponse) => {
            this.messageResponse.unshift(element);
          });
        }
      });
  }

  userProfile(){
    this.router.navigateByUrl(`/user/${this.selectedUser.receiverId}`)
  }

  returnUserProfile(user: UserViewModel): string{
    if(user.avatar.trim()){
      return user.avatar
    }else{
      if(user.gender == 'Male'){
        return 'assets/male-avatar.avif';
      } else if (user.gender == 'Female'){
        return 'assets/female-avatar.avif';
      } else{
        return 'assets/rather-not-say.avif';
      }
    }
  }

  getMessageTime(time: string): string{
    const now = new Date();
    const messageTime = new Date(time);
    const timeDifference = now.getTime() - messageTime.getTime();
    if (timeDifference < 24 * 60 * 60 * 1000) {
      const hours = messageTime.getHours();
      const minutes = messageTime.getMinutes();
      return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}`;
    } else {
      const month = messageTime.toLocaleString('default', { month: 'short' });
      const day = messageTime.getDate();
      return `${month} ${day.toString().padStart(2, '0')}`;
    }
  }

}
