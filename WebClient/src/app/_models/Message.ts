 
export interface Message {
  id: number;
  senderId: number;
  senderUsername: string;
  recipientId: number;
  recipientPhotoUrl: string;
  senderPhotoUrl: string;
  recipientUsername: string;
  content: string;
  dateRead?: Date;
  messageSent: Date;
}