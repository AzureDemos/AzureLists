import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Task } from '../models/task';
import { List } from '../models/list';
import { ListsService } from '../listsservice.service';
import { Observable } from 'rxjs';
@Component({
  selector: 'az-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {
 
  constructor(private route: ActivatedRoute, private router: Router, private listService: ListsService) { }

  listId: string;
  private sub: any;
  public list: List;
  public showCompleted: Boolean = false;
  public newTask: Task;
  public editListMode: Boolean = false;
  public listName: string = "";
  ngOnInit() {
  this.ReLoad();
  }

  public ReLoad(){
    console.log("Reload tasks")
    this.sub = this.route.params.subscribe(params => {
      this.listId = params['id']; 
      console.log("getting tasks for list " + this.listId);
      this.listService.getLists().subscribe(data => {
        this.list = data.filter((x) => x.Id === this.listId)[0];
        this.bindNewTask();
        if (this.list){
          this.listName = this.list.Name;
          if (!this.list.Tasks){
            this.list.Tasks = [];
          }
        }else{
          this.listName = "";
        }
      }, error => console.error(error)); 
    });
  }

  private bindNewTask(){
    this.newTask = { Id: null, Title: "", Notes: "", CompletedDate: null, DueDate: null, Important: false };
  }

  //UI Logic

  public toggleEditList(){
    console.log(this.editListMode)
    this.editListMode = !this.editListMode;
    console.log(this.editListMode)
  }

  public toggleCompleted () {
    this.showCompleted = !this.showCompleted;
  }

  public isVirtual(list: List){
    return list.Id === "important" || list.Id === "thisweek";
  }

  // List Service Actions
  public AddTask(){
    
    this.listService.addTaskToList(this.list, this.newTask).subscribe(result => {
      this.newTask = result;
      this.list.Tasks.push(this.newTask);
      this.bindNewTask();
    }, error => {  console.error(error); });;
  
  }

  
  public updateList(){
    console.log("Update List");
    this.list.Name = this.listName;
    this.listService.updateList(this.list).subscribe(result => {
      this.list = result;
      this.toggleEditList();
    }, error => {  console.error(error);});
  }

  public deleteList(){
    console.log("Delete List");
    this.listService.deleteList(this.list).subscribe(result => {
      console.log(result);
      this.router.routeReuseStrategy.shouldReuseRoute = function(){return false;} //force list view to re-load it's data
      this.router.navigate(['/lists']);
    }, error => {  console.error(error);});
  }

  public toggleTaskCompleted(task: Task, event){
    event.stopPropagation();
    if (task.CompletedDate){  
      task.CompletedDate = null; }
    else{ 
      task.CompletedDate = new Date(); 
    }
    
    this.listService.updateTask(task).subscribe(result => {
      task = result;
    }, error => {  console.error(error);});

    return false;
  }
  

  public toggleTaskImportant(task: Task, event){
    event.stopPropagation();
    task.Important = !task.Important;
    if (task.Id){ //otherwise we are toggling a new task
       this.listService.updateTask(task).subscribe(result => {
        task = result;
      }, error => {  console.error(error); });
    }
    return false;
  }


}
