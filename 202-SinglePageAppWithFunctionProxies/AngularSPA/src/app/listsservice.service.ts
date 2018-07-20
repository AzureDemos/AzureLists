import { Injectable } from '@angular/core';
import { Http,Headers, RequestOptions } from '@angular/http';
import { Observable, of } from 'rxjs';
import { List } from './models/list';
import { catchError, map, tap, publishReplay, refCount } from 'rxjs/operators';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import { Task } from './models/task';


@Injectable({
  providedIn: 'root'
})
export class ListsService {

  private url:string = 'https://azurelistsapi.azurewebsites.net';
  
  private lists: Observable<Array<List>>;
  private observable: Observable<List[]> ;
  constructor(private httpClient:HttpClient, private http: Http) {
   
  }

  clearCache() {
    this.lists = null;
  }

  //List Actions
  getLists() {
    if (!this.lists) {
      this.lists = this.httpClient.get<List[]>(this.url + '/api/lists')
        .pipe(
          map((res => this.ModifyList(res))),
          publishReplay(1),//cache the most recent value
          refCount(),//keep the observable alive for as long as there are subscribers
        );
    }
    return this.lists;
  }

  updateList(list: List) {
    console.log("Calling API to update list " + list.Id)
    var data = JSON.stringify(list);
    let httpOptions = { headers: new HttpHeaders({'Content-Type':  'application/json', })};
    this.clearCache(); 
    return this.httpClient.put<List>(this.url + '/api/lists/' + list.Id, data, httpOptions);
  }

  createList(list: List) {
    console.log("Calling API to create list ")
    var data = JSON.stringify(list);
    let httpOptions = { headers: new HttpHeaders({'Content-Type':  'application/json', })};
    this.clearCache(); 
    return this.httpClient.post<List>(this.url + '/api/lists', data, httpOptions);
  }

  deleteList(list: List) {
    console.log("Calling API to delete list " + list.Id)
    let httpOptions = { headers: new HttpHeaders({'Content-Type':  'application/json', })};
    this.clearCache(); 
    return this.httpClient.delete(this.url + '/api/lists/' + list.Id, httpOptions);
  }

  addTaskToList(list: List, task: Task){
    
    console.log("Calling API to update list " + list.Id)
    var data = JSON.stringify(task);
    let httpOptions = { headers: new HttpHeaders({'Content-Type':  'application/json', })};
    this.clearCache();
    return this.httpClient.post<Task>(this.url + '/api/lists/' + list.Id + "/Tasks", data, httpOptions);
  }

  //Task Actions
  updateTask(task: Task) {
    console.log("Calling API to update task " + task.Id)
    var data = JSON.stringify(task);
    let httpOptions = { headers: new HttpHeaders({'Content-Type':  'application/json', })};
    this.clearCache(); //"This Weeks" or "Important" virtual lists could have changed from this
    return this.httpClient.put<Task>(this.url + '/api/tasks/' + task.Id, data, httpOptions);
  }

  deleteTask(task: Task) {
    console.log("Calling API to delete task " + task.Id)
    let httpOptions = { headers: new HttpHeaders({'Content-Type':  'application/json', })};
    this.clearCache(); 
    return this.httpClient.delete(this.url + '/api/tasks/' + task.Id, httpOptions);
  }


  // ************************** Helpers *************************************

  // NOTE - We could get this via the API, but for this demo with very little data 
  // we are just calculating it. This saves us making additional requests
  ModifyList(lists: List[]) {
    if (lists && lists.length > 0){
      
      var thisWeeksTasks = this.addThisWeeksTasksVirtualList(lists);
      var importantTasks = this.addImportantTasksVirtualList(lists);
      lists.splice(0, 0, thisWeeksTasks);
      lists.splice(0, 0, importantTasks);

      //As the API is basic functionality, the lists are not in any order
      //Therefore, lets check for the default 'inbox', and put it to the top
      var inboxMatches = lists.filter((fil) => fil.Name === "Inbox");
      if (inboxMatches && inboxMatches.length > 0){
        var inbox = inboxMatches[0];
        var idx = lists.indexOf(inbox);
        lists.splice(idx, 1);
        lists.splice(0, 0, inbox);
      }
      
    }
    return lists;
  }

  private addThisWeeksTasksVirtualList(lists: List[]) {
    var matches = lists.filter((fil) => fil.Name === "This Week");
    if (matches && matches.length > 0){
      var match = matches[0];
      lists.splice(lists.indexOf(match), 1);
    }
    //Create Virtual List Called This Week
    var thisweeksTasks:Task[] = [];
    lists.forEach((x) => {
      if (x.Tasks && x.Tasks.length > 0){
        var thisweeksMatches = x.Tasks.filter((fil) => fil.Important)
        if (thisweeksMatches && thisweeksMatches.length > 0){
          thisweeksMatches.forEach((i) => {
            thisweeksTasks.push(i);
          })
        }
      }
    });
    if (!thisweeksTasks) {
      thisweeksTasks = [];
    }
    var thisWeek : List =
    {
        Id: "thisweek",
        Name: "This Week",
        Tasks: thisweeksTasks,
        IsVirtual: true
    };
    return thisWeek;
  }

  private addImportantTasksVirtualList(lists: List[]) {
    var matches = lists.filter((fil) => fil.Name === "Important");
    if (matches && matches.length > 0){
      var match = matches[0];
      lists.splice(lists.indexOf(match), 1);
    }
    //Create Virtual List Called Important
    var importantTasks:Task[] = [];
    lists.forEach((x) => {
      if (x.Tasks && x.Tasks.length > 0){
        var importantsMatches = x.Tasks.filter((fil) => fil.Important === true)
        if (importantsMatches && importantsMatches.length > 0){
          importantsMatches.forEach((i) => {
            importantTasks.push(i); 
          })
        }
      }
    });
    if (!importantTasks) {
      importantTasks = [];
    }
    var important : List =
    {
        Id: "important",
        Name: "Important",
        Tasks: importantTasks,
        IsVirtual: true
    };
    return important;
  }

}


