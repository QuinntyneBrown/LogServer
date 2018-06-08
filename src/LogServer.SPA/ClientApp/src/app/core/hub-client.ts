import { Injectable, NgZone } from "@angular/core";
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";
import { Subject } from "rxjs";

@Injectable()
export class HubClient {
  public events: Subject<any> = new Subject();

  constructor(
    private _ngZone: NgZone) {
  }

  private _connection: HubConnection;

  private _connect: Promise<any>;

  public connect(): Promise<any> {

    if (this._connect)
      return this._connect;

    this._connect = new Promise((resolve) => {
      
      this._connection = this._connection || new HubConnectionBuilder()
        .withUrl(`http://localhost:47270/hub`)
        .build();

      this._connection.on("message", (value) => {
        this._ngZone.run(() => this.events.next(value));
      });

      this._connection.start().then(() => resolve());
    });

    return this._connect;
  }

  public disconnect() {
    if (this._connection) {
      this._connection.stop();
      this._connect = null;
      this._connection = null;
    }
  }
}
