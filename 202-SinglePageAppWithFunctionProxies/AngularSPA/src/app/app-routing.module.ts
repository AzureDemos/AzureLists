import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListsmodModule } from './listsmod/listsmod.module';

const routes: Routes = [
  
{
  path:'', 
  pathMatch: 'full', 
  redirectTo: 'lists'
}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
