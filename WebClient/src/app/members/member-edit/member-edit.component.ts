import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_sevices/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editForm') editForm:NgForm | undefined
  // tells browser to prevent to another page and shows confirm box
  @HostListener('window:beforeunload',['$event']) unloadNotification($event:any){
    if(this.editForm?.dirty){
      $event.returnValue=true;
    }
  }
  member:Member | undefined;
  user:User |null=null;
  

  constructor(private accountService:AccountService,private membersService:MembersService,private tostr:ToastrService) {
    
       this.accountService.currentUser$.pipe(take(1)).subscribe({
        next:user=>{
          console.log("member edit user "+user?.userName);
          this.user=user
        }
       })
  }
  ngOnInit(): void {
   
    this.loadMember();

  }

    loadMember(){
      if(!this.user) return;
      this.membersService.getMember(this.user.userName).subscribe({
        next:member=>{
          console.log("member edit "+member);
          this.member=member
        }
      })
    }
     updateMember(){
 this.membersService.updateMember(this.editForm?.value).subscribe({
  next:_=>{
    this.tostr.success('profile Update Successfully');
    this.editForm?.reset(this.member);
  }
 })
     }

}
