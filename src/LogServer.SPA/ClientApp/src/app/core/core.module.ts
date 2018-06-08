import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HubClient } from './hub-client';
import { SharedModule } from '../shared/shared.module';

const declarations = [
];

const providers = [
  HubClient
];

@NgModule({
  declarations,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,

    SharedModule
  ],
  providers
})
export class CoreModule { }
