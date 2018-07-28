import { Component, ViewChild } from '@angular/core';
import { HubClient } from './core/hub-client';
import { Observable, BehaviorSubject } from 'rxjs';
import { LogService } from './logs/log.service';
import { map } from 'rxjs/operators';
import { merge } from 'rxjs';
import { IgxGridComponent } from 'igniteui-angular';

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

  @ViewChild("grid")
  public grid: IgxGridComponent;

  public clearSearch() {
    this.searchText = "";
    this.grid.clearSearch();
  }

  public searchKeyDown(ev) {
    if (ev.key === "Enter" || ev.key === "ArrowDown" || ev.key === "ArrowRight") {
      ev.preventDefault();
      this.grid.findNext(this.searchText, this.caseSensitive);
    } else if (ev.key === "ArrowUp" || ev.key === "ArrowLeft") {
      ev.preventDefault();
      this.grid.findPrev(this.searchText, this.caseSensitive);
    }
  }

  public updateSearch() {
    this.caseSensitive = !this.caseSensitive;
    this.grid.findNext(this.searchText, this.caseSensitive);
  }
  public searchText: string = "";
  public caseSensitive: boolean = false;

  public messages$: BehaviorSubject<any[]> = new BehaviorSubject([]);

  public mergedMessages$: Observable<any[]>;
}
