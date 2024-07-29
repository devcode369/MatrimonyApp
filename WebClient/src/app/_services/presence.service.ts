import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
hubsUrl=environment.hubsurl;

private hubConnection?:HubConnection;
private onlineUsersSource=new BehaviorSubject<string[]>([]);
onlineUsers$=this.onlineUsersSource.asObservable();

  constructor(private toastr:ToastrService) { }

  createHubConnection(user:User){
    console.log(user);
    this.hubConnection=new HubConnectionBuilder().withUrl(this.hubsUrl+'presence',{
      accessTokenFactory:()=>user.token
     //skipNegotiation: true,
    //transport: signalR.HttpTransportType.WebSockets

    })
    //.configureLogging(signalR.LogLevel.Information)

    .withAutomaticReconnect()
    .build();

    
    console.log(this.hubConnection);
    console.log(JSON.stringify(this.hubConnection));


    this.hubConnection.start().catch(error=>console.log(error));

    this.hubConnection.on('UserIsOnline',username=>{
      this.toastr.info(username+' has connectd');
    });

    this.hubConnection.on('UserIsOffline',username=>{
      this.toastr.warning(username+' has disconnected')
    })
    
    this.hubConnection.on('GetOnlineUsers',username=>{
      this.onlineUsersSource.next(username);
    })
  }

  stopHubConnection(){
    this.hubConnection?.stop().catch(error=>console.log(error));
  }
}
