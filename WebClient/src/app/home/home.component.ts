import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
registerMode=false;
users:any;
constructor(private http: HttpClient) {
  
  
}
  ngOnInit(): void {
  this.getUsers();
  }
   ngOnInt():void{

   }
   registerToggle(){
    this.registerMode=!this.registerMode;
   }

   getUsers(){
    this.http.get('https://localhost:5260/api/Users').subscribe({
      next: response=>this.users=response,
      error: error=>console.log(error),
      complete:()=>console.log('request completed')
  
    })
  }
}
