import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
hubsUrl=environment.hubsurl;

private hubConnection?:HubConnection;
private onlineUsersSource=new BehaviorSubject<string[]>([]);
onlineUsers$=this.onlineUsersSource.asObservable();

  constructor(private toastr:ToastrService,private router:Router) { }

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
      this.onlineUsers$.pipe(take(1)).subscribe({
        next:usernames=>this.onlineUsersSource.next([...usernames,username])
      })
    });

    this.hubConnection.on('UserIsOffline',username=>{
      this.onlineUsers$.pipe(take(1)).subscribe({
        next:usernames=>this.onlineUsersSource.next(usernames.filter(x=>x !==username))
      })
    })
    
    this.hubConnection.on('GetOnlineUsers',username=>{
      this.onlineUsersSource.next(username);
    })

    this.hubConnection.on('NewMessageReceived', ({ username, knownAs, photoUrl }) => {
      this.toastr.info(`<span class="chat-img"><img src="${photoUrl}" alt="image of user" class="rounded-circle"></span> ${knownAs} has sent you a new message! Click me to see it`)
        .onTap
        .pipe(take(1))
        .subscribe({
          next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
        });
    });
    // this.hubConnection.on('NewMessageReceived', ({ username, knownAs, photoUrl }) => {
    //   this.toastr.info(
    //     `<div style="display: flex; align-items: center;">
    //       <img src="${photoUrl}" alt="image of user" class="rounded-circle" style="width: 10px; height: 10px; margin-right: 10px;">
    //       <span>${knownAs} has sent you a new message! Click me to see it</span>
    //     </div>`,
    //     '',
    //     {
    //       enableHtml: true
    //     }
    //   ).onTap
    //     .pipe(take(1))
    //     .subscribe({
    //       next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
    //     });
    // });
    
  }

  stopHubConnection(){
    this.hubConnection?.stop().catch(error=>console.log(error));
  }
}
