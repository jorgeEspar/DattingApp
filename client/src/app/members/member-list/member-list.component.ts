import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];

  totalItemsMembers = 0;
  totalPagesMembers = 0;
  numItemsPerPage = 5;
  mostrando_paginacion = "";

  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void{ 
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if(this.userParams) {
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if(response.result && response.pagination){
            this.members = response.result;
            this.pagination = response.pagination;

            this.totalItemsMembers= response.pagination.totalItems;
            this.totalPagesMembers= response.pagination.totalPages;
            this.numItemsPerPage= response.pagination.itemsPerPage;
            var cadena="Mostrando del ";
            if(response.pagination.totalItems>0){
              cadena += ((response.pagination.currentPage-1) * response.pagination.itemsPerPage) + 1 + " al ";
              if(((response.pagination.currentPage-1) * response.pagination.itemsPerPage + response.pagination.itemsPerPage) < response.pagination.totalItems)
              {
                cadena += ((response.pagination.currentPage-1) * response.pagination.itemsPerPage + response.pagination.itemsPerPage).toString();
              }else{
                cadena += response.pagination.totalItems.toString();
              }
              cadena += " de " + response.pagination.totalItems.toString() + " usuarios";
            }else{
              cadena="No hay usuarios seleccionados con los criterios indicados.";
            }

            this.mostrando_paginacion=cadena;
          }
        }
      })
    }
  }
  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
