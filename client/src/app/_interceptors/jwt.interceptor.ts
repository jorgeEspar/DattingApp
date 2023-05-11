import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { AccountService } from '../_services/account.service';
import { Observable, take } from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.accountService.currenUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) {
          request = request.clone({
            setHeaders:{
              //Authorization: 'Bearer ${user.token}' -> JorgeEsp -> esto no ha funcionado: ${user.token}
              Authorization: 'Bearer ' + user.token
            }
          })
        }
      }
    })

    return next.handle(request);
  }
}
