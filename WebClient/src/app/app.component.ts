//import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './-models/users';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'MatrimonyApp';
  //users:any;
 constructor(private accountService:AccountService){

 }
  ngOnInit(): void {
   // this.getUsers();
    this.setCurrentUser();
  }

  ///Moved to home Component so removev Http client from constrcut and user any from int 
  // getUsers(){
  //   this.http.get('https://localhost:5260/api/Users').subscribe({
  //     next: response=>this.users=response,
  //     error: error=>console.log(error),
  //     complete:()=>console.log('request completed')
  
  //   })
  // }
   setCurrentUser(){
    //const user:User =JSON.parse(localStorage.getItem('user')!)
     const userString=localStorage.getItem('user');
     if(!userString) return;
     const user:User=JSON.parse(userString);
     this.accountService.setCurrentUser(user);
  }
}
