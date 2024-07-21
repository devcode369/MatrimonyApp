import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { DefaultNoComponentGlobalConfig, ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';

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

  registerForm:FormGroup =new FormGroup({});

 
  constructor(private accountService:AccountService,private toaster:ToastrService,private formBuilder:FormBuilder) {
     
    
  }
  ngOnInit(): void {
  this.initializeForm();
  }

initializeForm(){

  // 408 the below one replaced by formbuilder 
  // this.registerForm=new FormGroup({
  //   username:new FormControl('',Validators.required),
  //   password:new FormControl('',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]),
  //   confirmPassword:new FormControl('',[Validators.required,this.matchValues('password')]),
  // });

  this.registerForm=this.formBuilder.group({
    username:['',Validators.required],
    password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
    confirmPassword:['',[Validators.required,this.matchValues('password')]],
  });
  this.registerForm.controls['password'].valueChanges.subscribe({

    next:()=>this.registerForm.controls['confirmPassword'].updateValueAndValidity()
  })
}

matchValues(matchTo:string):ValidatorFn{
   return (control:AbstractControl)=>{

    return control.value===control.parent?.get(matchTo)?.value?null:{notMatching:true}
   }
}
  register(){
    console.log(this.registerForm?.value);
  //  this.accountService.register(this.model).subscribe({
  // //   next:response=>{

  // // console.log(response);
  // // this.cancel();
  // //  },
  // next:()=>{

    
  //   this.cancel();
  //    },
  //  //error: error => console.log(error)
  //  error: error => this.toaster.error(error.error)
  //  })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }
      
}
