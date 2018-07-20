import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ListsmodRoutingModule } from './listsmod-routing.module';
import { ListsComponent } from '../lists/lists.component';
import { TasksComponent } from '../tasks/tasks.component';
import { TaskComponent } from '../task/task.component';
import { ListsService } from '../listsservice.service';
import { ListsfilterPipe } from '../shared/listsfilter.pipe';
import { FormsModule , ReactiveFormsModule } from '@angular/forms';
import { MyDatePickerModule } from 'mydatepicker';
@NgModule({
  imports: [
    CommonModule,
    ListsmodRoutingModule,
    FormsModule,
    MyDatePickerModule
  ],
  providers: [ListsService],
  declarations: [ListsComponent, TasksComponent, TaskComponent,ListsfilterPipe]
})
export class ListsmodModule { }
