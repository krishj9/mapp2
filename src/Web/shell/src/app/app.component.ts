import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  template: `
    <mat-toolbar color="primary">
      <mat-toolbar-row>
        <span>MAPP - Business Application</span>
        <span class="spacer"></span>
        <button mat-button routerLink="/planning">Planning</button>
        <button mat-button routerLink="/observations">Observations</button>
        <button mat-button routerLink="/users">Users</button>
        <button mat-button routerLink="/reports">Reports</button>
      </mat-toolbar-row>
    </mat-toolbar>

    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav #drawer class="sidenav" fixedInViewport
          [attr.role]="'navigation'"
          [mode]="'over'"
          [opened]="false">
        <mat-toolbar>Menu</mat-toolbar>
        <mat-nav-list>
          <a mat-list-item routerLink="/planning">
            <mat-icon>assignment</mat-icon>
            <span>Planning</span>
          </a>
          <a mat-list-item routerLink="/observations">
            <mat-icon>visibility</mat-icon>
            <span>Observations</span>
          </a>
          <a mat-list-item routerLink="/users">
            <mat-icon>people</mat-icon>
            <span>User Management</span>
          </a>
          <a mat-list-item routerLink="/reports">
            <mat-icon>assessment</mat-icon>
            <span>Reports</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <div class="toolbar-spacer"></div>
        <main class="main-content">
          <router-outlet></router-outlet>
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .spacer {
      flex: 1 1 auto;
    }
    
    .sidenav-container {
      height: calc(100vh - 64px);
    }
    
    .sidenav {
      width: 200px;
    }
    
    .toolbar-spacer {
      height: 8px;
    }
    
    .main-content {
      padding: 20px;
      height: calc(100vh - 84px);
      overflow: auto;
    }
  `]
})
export class AppComponent {
  title = 'MAPP Shell';
}
