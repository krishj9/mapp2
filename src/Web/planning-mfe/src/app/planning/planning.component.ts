import { Component } from '@angular/core';

@Component({
  selector: 'app-planning',
  template: `
    <div class="planning-container">
      <mat-card class="header-card">
        <mat-card-header>
          <mat-card-title>
            <mat-icon>assignment</mat-icon>
            Planning Management
          </mat-card-title>
          <mat-card-subtitle>
            Create, manage, and track your plans with AI-powered insights
          </mat-card-subtitle>
        </mat-card-header>
      </mat-card>
      
      <div class="content-area">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: [`
    .planning-container {
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
export class PlanningComponent { }
