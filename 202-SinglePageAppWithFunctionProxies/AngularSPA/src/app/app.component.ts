import { Component } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';

@Component({
  selector: 'az-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'az';

  constructor(updates: SwUpdate){
    updates.available.subscribe(event => {
      updates.activateUpdate().then(() => document.location.reload());
    })
  }
}
