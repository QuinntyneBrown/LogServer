import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Log } from "./log.model";

@Injectable()
export class LogService {
  constructor(
    private _client: HttpClient
  ) { }

  public get(): Observable<Array<Log>> {
    return this._client.get<{ logs: Array<Log> }>(`http://localhost:45121/api/logs`)
      .pipe(map(x => x.logs));
  }
}
