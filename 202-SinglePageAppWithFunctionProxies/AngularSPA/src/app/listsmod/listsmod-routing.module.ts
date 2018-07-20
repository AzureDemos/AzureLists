import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListsComponent } from '../lists/lists.component';
import { TasksComponent } from '../tasks/tasks.component';
import { TaskComponent } from '../task/task.component';


const routes: Routes = [
  { 
    path: 'lists',  
    component: ListsComponent,
    children: [
      {
          path: ":id",
          component: TasksComponent,
          children: [
            {
                path: "task/:task-id",
                component: TaskComponent
            }
          ]
      }
    ]
  }
]


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ListsmodRoutingModule { }
