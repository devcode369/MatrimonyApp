import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/Message';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl=environment.apiUrl;
  hubUrl=environment.hubsurl;
  private hubConnection?:HubConnection;
  private messageThreadSource=new BehaviorSubject<Message[]>([]);
  messageThread$=this.messageThreadSource.asObservable();
  
  constructor(private http:HttpClient) {

   }
   createHubConnection(user:User,otherUsername:string)
   {
       this.hubConnection=new HubConnectionBuilder()
       .withUrl(this.hubUrl+'message?user='+ otherUsername,{
        accessTokenFactory:()=>user.token,
        transport: HttpTransportType.WebSockets
       })
       .withAutomaticReconnect()
       .build();

       this.hubConnection.start().catch(error=>console.log(error));
       this.hubConnection.on('ReceiveMessageThread',message=>{
          this.messageThreadSource.next(message);
       })

       this.hubConnection.on('NewMessage',message=>{
          this.messageThread$.pipe(take(1)).subscribe({
            next:messages=>{
              this.messageThreadSource.next([...messages,message])
            }
          })
       })

   }

   stopHubConnection()
   {
    if(this.hubConnection)
    {
       this.hubConnection.stop();
    }
   }
    getMessages(pageNumber:number,pageSize:number,container:string){
      let params=getPaginationHeaders(pageNumber,pageSize);
      params=params.append('Container',container);
      return getPaginatedResult<Message[]>(this.baseUrl+'messages',params,this.http);
    }
    getMessageThread(username:string)
    {
      return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+username);
    }

    async sendMessage(username:string,content:string){
     return this.hubConnection?.invoke('SendMessage',{recipientUsername: username,content})
     .catch(error=>console.log(error));
    }
    deleteMessage(id:number){
      return this.http.delete(this.baseUrl+'messages/'+id);
    }
  }