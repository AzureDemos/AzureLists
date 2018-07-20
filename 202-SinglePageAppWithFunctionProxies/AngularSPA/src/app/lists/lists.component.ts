import { Component, OnInit } from '@angular/core';
import { List } from '../models/list';
import { ListsService } from '../listsservice.service';
import { Observable } from 'rxjs';
import { Task } from '../models/task';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'az-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  public lists:List[];
  public sideBarCss: string ="";
  private sub: any;
  public newListMode: Boolean = false;
  constructor( private route: ActivatedRoute,private router: Router, 
  private listService: ListsService) { }
  public newList: List = { Id: null, Name: "", IsVirtual:false, Tasks:[]};

  ngOnInit() {
    this.listService.getLists().subscribe(data => {
      this.bindNewList();
      this.lists = data
      if (!this.route.firstChild) {
        this.router.navigate(['/lists', this.lists[0].Id]);
      }
     
    }, error => console.error(error)); 
  }

  private bindNewList(){
    this.newList = { Id: null, Name: "", IsVirtual:false, Tasks:[]};
  }
  public createList(){
    console.log("Update Task");
    
    this.listService.createList(this.newList).subscribe(result => {
      this.lists.push(result);
      this.bindNewList();
      this.toggleNewList();
    }, error => {  console.error(error);});
  }
  
  public toggleNewList(){
    this.newListMode = !this.newListMode;
  }

  public toggleSideBar(){
    if (this.sideBarCss === ""){
      this.sideBarCss = "sidebar-small";
    }
    else {
      this.sideBarCss = "";
    }
  }

  public getIcon(list: List){
    if (list.Name === "Inbox"){
      return "fa-inbox";
    }
    else if (list.Name === "Important"){
      return "fa-star";
    }
    if (list.Name === "This Week"){
      return "fa-calendar-alt";
    }
    else{
      return "fa-list";
    }
  }
}
