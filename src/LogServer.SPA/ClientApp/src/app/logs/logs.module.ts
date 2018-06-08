import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LogService } from './log.service';
import { HttpClientModule } from '@angular/common/http';

const declarations = [
];

const providers = [
  LogService
];

@NgModule({
  declarations: declarations,
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule	
  ],
  providers,
})
export class LogsModule { }
