import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Http } from '@angular/http';
import { Task } from '../models/task';
import { ListsService } from '../listsservice.service';
import {IMyDpOptions, MyDatePicker, IMyDateModel} from 'mydatepicker';
import { EVENT_MANAGER_PLUGINS } from '@angular/platform-browser';

@Component({
  selector: 'az-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css']
})
export class TaskComponent implements OnInit {

  constructor(private route: ActivatedRoute, private listService: ListsService,private router: Router) { }

  public task: Task;
  listId: string;
  taskId: string;
  private sub: any;
  public datePickerOptions: IMyDpOptions = {dateFormat: 'dd/mmm/yyyy'};
  public datePickerModel: any;
  ngOnInit() {
    
    this.route.parent.params.subscribe(params => {
      this.listId = params['id']; 
    });

    this.sub = this.route.params.subscribe(params => {
      this.taskId = params['task-id']; 
      console.log("getting task for list " + this.listId + " task " + this.taskId);
      this.listService.getLists().subscribe(data => {
        let lists = data;
        let list = lists.filter((x) => x.Id === this.listId)[0];
        this.task = list.Tasks.filter((x) => x.Id === this.taskId)[0];
        this.bindDatePicker();
      }, error => console.error(error)); 
    });

  }

  bindDatePicker(){
    if (this.task.DueDate){
      let d = new Date(this.task.DueDate);
      this.datePickerModel = { date: { year: d.getFullYear(), month: d.getMonth() + 1, day: d.getDate() } };
    } else{
      this.datePickerModel = null;
    }
  }

  onDateChanged(event: IMyDateModel) {    // event properties are: event.date, event.jsdate, event.formatted and event.epoc
    if (event){
      this.task.DueDate = event.jsdate;
      console.log("Due Date Changed to: " +  this.task.DueDate);
    } else {
      this.task.DueDate = null;
    }
  }

  public updateTask(){
    console.log("Update Task");
    this.listService.updateTask(this.task).subscribe(result => {
      this.task = result;
    }, error => {  console.error(error);});
  }

  public deleteTask(){
    console.log("Delete Task");
    this.listService.deleteTask(this.task).subscribe(result => {
      this.router.routeReuseStrategy.shouldReuseRoute = function(){return false;} //force list view to re-load it's data
      this.router.navigate(['/lists', this.listId]);
    }, error => {  console.error(error);});
  }

  public toggleTaskCompleted(){
    if (this.task.CompletedDate){  
      this.task.CompletedDate = null; }
    else{ 
      this.task.CompletedDate = new Date(); 
    }
    
    this.listService.updateTask(this.task).subscribe(result => {
      this.task = result;
    }, error => {  console.error(error);});
  }
  

  public toggleTaskImportant(){
    this.task.Important = !this.task.Important;
    if (this.task.Id){ //otherwise we are toggling a new task
       this.listService.updateTask(this.task).subscribe(result => {
        this.task = result;
      }, error => {  console.error(error); });
    }
  }


}
