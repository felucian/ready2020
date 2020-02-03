import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { BookComponent } from './components/book/book.component';
import { StatefulbookComponent } from './components/statefulbook/statefulbook.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        BookComponent,
        FetchDataComponent,
        HomeComponent,
        StatefulbookComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'book', component: BookComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'statefulbook', component: StatefulbookComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
