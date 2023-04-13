import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
  model: any={}
  currentUser$: Observable<User | null> =  of(null); // propiedad para indicar si el user está logado

  constructor(public accountService: AccountService) { }

  ngOnInit(): void{
    this.currentUser$ = this.accountService.currenUser$;
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
      },
      error: error => console.log(error) 
    })
  }

  logout() {
    this.accountService.logout();
  }
}
