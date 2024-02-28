import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // calling get user dettail from home component (parent)
 // no need 
  //@Input() usersFromComponent:any;
  @Output() cancelRegister =new EventEmitter();  

  model:any={}
 
  constructor(private accountService:AccountService,private toaster:ToastrService) {
     
    
  }
  ngOnInit(): void {
  
  }

  register(){
   this.accountService.register(this.model).subscribe({
  //   next:response=>{

  // console.log(response);
  // this.cancel();
  //  },
  next:()=>{

    
    this.cancel();
     },
   //error: error => console.log(error)
   error: error => this.toaster.error(error.error)
   })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }
      
}
