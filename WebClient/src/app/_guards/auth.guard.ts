import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const AuthGuard: CanActivateFn = (route, state) => {
  const accountService=inject(AccountService);
  const toaster=inject(ToastrService);
  //return true;

  // currentUser$ is observable so use pipe
  return accountService.curentUser$.pipe(
    map(user=>{
      if(user)return true;
       else{
        toaster.error('you shall not pass!');
        return false;
       }

    })
  )
};
