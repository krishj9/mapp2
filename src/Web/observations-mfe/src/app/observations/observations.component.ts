import { Component } from '@angular/core';

@Component({
  selector: 'app-observations',
  template: `
    <div class="observations-container">
      <mat-card class="header-card">
        <mat-card-header>
          <mat-card-title>
            <mat-icon>visibility</mat-icon>
            Observations Management
          </mat-card-title>
          <mat-card-subtitle>
            Collect, validate, and analyze observation data with AI-powered insights
          </mat-card-subtitle>
        </mat-card-header>
      </mat-card>
      
      <div class="content-area">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: [`
    .observations-container {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }
    
    .header-card {
      margin-bottom: 20px;
    }
    
    .header-card mat-card-title {
      display: flex;
      align-items: center;
      gap: 8px;
    }
    
    .content-area {
      min-height: 500px;
    }
  `]
})
export class ObservationsComponent { }
