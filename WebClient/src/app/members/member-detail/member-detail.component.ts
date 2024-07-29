import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_sevices/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/Message';
import { subscribeOn, take } from 'rxjs';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit,OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;

  member: Member ={} as Member;
  images: GalleryItem[] = [];
  activateTab?: TabDirective;
  messages: Message[] = [];
  user?:User;
  constructor(private accountService:AccountService,private route: ActivatedRoute, 
    private messageService: MessageService ,public presenceService:PresenceService
  
  ) {

     this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=>{
        if(user) this.user=user;
      }
     })
  }
  ngOnDestroy(): void {
    
   this.messageService.stopHubConnection();
  }

  onTabActivated(data: TabDirective) {
    this.activateTab = data;
    if (this.activateTab.heading === "Messages" && this.user) {
      console.log("enter avtivated tab ");
      console.log("memberdetailcomponent OnActivatetab member user " + this.member.userName);
      console.log("memberdetailcomponent OnActivatetab user " + this.user);
      this.messageService.createHubConnection(this.user,this.member.userName);
     // this.loadMessages();
    }else{
      this.messageService.stopHubConnection();
    }
  }
  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => {
          this.messages = messages
        }
      })
    }
  }

  ngOnInit(): void {

    this.route.data.subscribe(
      {
        next:data=>this.member=data['member']
      }
    )
  
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })
    this.getImages();
  }
  // loadMember() {
  //   const username = this.route.snapshot.paramMap.get('username');
  //   if (!username) return;
  //   this.memberService.getMember(username).subscribe({
  //     next: member => {
  //       this.member = member,
  //         this.getImages()
  //     }

  //   })
  // }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

  getImages() {
    if (!this.member) return
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
}
