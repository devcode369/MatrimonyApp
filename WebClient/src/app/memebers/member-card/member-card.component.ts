import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { PresenceService } from 'src/app/_services/presence.service';
import { MembersService } from 'src/app/_sevices/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  encapsulation:ViewEncapsulation.None
})
export class MemberCardComponent implements OnInit {
 //member list(parent) to member card (child) ,each member have each card
 @Input() member:Member | undefined;

 
  constructor(private memberServices:MembersService,private toaster:ToastrService,public presenceService:PresenceService) {
   
    
  }
  ngOnInit(): void {

  }

  addLike(member:Member)
  {
    this.memberServices.addLike(member.userName).subscribe({
      next:()=>this.toaster.success('you have liked'+member.knownAs)
    })
  }

}
