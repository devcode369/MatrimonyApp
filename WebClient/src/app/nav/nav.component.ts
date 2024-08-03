import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

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
  constructor(public  accountService: AccountService,private route:Router,private toaster:ToastrService) {
 
    
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
    
    next:_=>{
      this.route.navigateByUrl('/members')
      this.model={};
    }
    //, have interceptor so dont need   
   // error : error=>console.log(error)
   //error : error=>this.toaster.error(error.error)
  })
  }

  logout(){
    this.accountService.logout();
    this.route.navigateByUrl('/')
    
  }

}
