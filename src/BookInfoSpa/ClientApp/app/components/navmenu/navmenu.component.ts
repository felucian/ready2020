import { Component } from '@angular/core';
declare var isResilient: boolean;

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {

    public isResilient: boolean = false;
    ngOnInit() {
        this.isResilient = isResilient;
    }
}
