import { Component } from '@angular/core';
import { HubClient } from './core/hub-client';
import { Observable, BehaviorSubject } from 'rxjs';
import { LogService } from './logs/log.service';
import { map } from 'rxjs/operators';
import { merge } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(
    private _hubClient: HubClient,
    private _logService: LogService
  ) {

    this._hubClient.connect();

    this._hubClient.events
      .subscribe(x => {
        this.messages$.next([x,...this.messages$.value]);
      });

    this._logService.get().subscribe(x => this.messages$.next(x.sort((a,b) => b.logId - a.logId)));
    
  }

  public messages$: BehaviorSubject<any[]> = new BehaviorSubject([]);

  public mergedMessages$: Observable<any[]>;
}
