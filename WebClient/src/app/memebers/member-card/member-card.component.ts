import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  encapsulation:ViewEncapsulation.None
})
export class MemberCardComponent implements OnInit {
 //member list(parent) to member card (child) ,each member have each card
 @Input() member:Member | undefined;
  ngOnInit(): void {

  }

}
