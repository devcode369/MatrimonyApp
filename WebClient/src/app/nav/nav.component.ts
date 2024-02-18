import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../-models/users';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model:any={};
  //loggedIn=false;   // no flag required instead used below but this also noisy we can 
  //directly use AccountService directly in our template
  currentUsers$:Observable<User|null>=of(null)

  /**
   *
   */
  constructor(public  accountService: AccountService) {
 
    
  }
  ngOnInit(): void {
 //this.getCurrentUser();
 // directly use AccountService directly in our template
 // this.currentUsers$=this.accountService.curentUser$;
  }

  // bestway we can use Async pipe so auto subscribe & SUBSCRIBE
/*   getCurrentUser(){
    this.accountService.curentUser$.subscribe({
      next:user=>this.loggedIn=!!user,//will convert user to bool 
      error:error=>console.log(error)
    })
  } */


  login(){
  this.accountService.login(this.model).subscribe({
    next:response=>{
      console.log(response);
     //this.loggedIn=true;
    },
    error : error=>console.log(error)
  })
  }

  logout(){
    this.accountService.logout();
   //this.loggedIn=false;
  }

}
