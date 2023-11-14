import { UserViewModel } from "./user-view-model";

export interface ChatHeadViewModel {
  receiverId: string;
  lastMessage: string;
  isMessageRead: boolean;
  receiverFirstName: string;
  receiverLastName: string;
  chatHeadId: string;
  user: UserViewModel
}

export interface AddMessageViewModel {
  chatId: string;
  senderId: string;
  receiverId: string;
  chatHeadId: string;
  message: string;
}

export interface  MessageResponse {
  chatId: string;
  senderId: string;
  senderUser: any;
  receiverId: string;
  receiverUser: any;
  message: string;
  sentTime: string;
  isMessageRead: boolean;
  chatHead: ChatHeadViewModel;
  chatHeadId: string;
}
