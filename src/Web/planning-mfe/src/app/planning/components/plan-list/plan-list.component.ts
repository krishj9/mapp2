import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PlanningService } from '../../services/planning.service';

@Component({
  selector: 'app-plan-list',
  template: `
    <div class="plan-list-container">
      <div class="actions-bar">
        <button mat-raised-button color="primary" (click)="createPlan()">
          <mat-icon>add</mat-icon>
          New Plan
        </button>
        <button mat-button (click)="getAISuggestions()">
          <mat-icon>psychology</mat-icon>
          AI Suggestions
        </button>
      </div>

      <mat-card class="plans-card">
        <mat-card-header>
          <mat-card-title>Your Plans</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div *ngIf="loading" class="loading-container">
            <mat-spinner></mat-spinner>
          </div>
          
          <mat-table [dataSource]="plans" *ngIf="!loading">
            <ng-container matColumnDef="title">
              <mat-header-cell *matHeaderCellDef>Title</mat-header-cell>
              <mat-cell *matCellDef="let plan">{{plan.title}}</mat-cell>
            </ng-container>

            <ng-container matColumnDef="status">
              <mat-header-cell *matHeaderCellDef>Status</mat-header-cell>
              <mat-cell *matCellDef="let plan">
                <span class="status-badge" [class]="'status-' + plan.status">
                  {{plan.status}}
                </span>
              </mat-cell>
            </ng-container>

            <ng-container matColumnDef="priority">
              <mat-header-cell *matHeaderCellDef>Priority</mat-header-cell>
              <mat-cell *matCellDef="let plan">{{plan.priority}}</mat-cell>
            </ng-container>

            <ng-container matColumnDef="created">
              <mat-header-cell *matHeaderCellDef>Created</mat-header-cell>
              <mat-cell *matCellDef="let plan">{{plan.created | date:'short'}}</mat-cell>
            </ng-container>

            <ng-container matColumnDef="actions">
              <mat-header-cell *matHeaderCellDef>Actions</mat-header-cell>
              <mat-cell *matCellDef="let plan">
                <button mat-icon-button (click)="viewPlan(plan.id)">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button (click)="editPlan(plan.id)">
                  <mat-icon>edit</mat-icon>
                </button>
              </mat-cell>
            </ng-container>

            <mat-header-row *matHeaderRowDef="displayedColumns"></mat-header-row>
            <mat-row *matRowDef="let row; columns: displayedColumns;" 
                     (click)="viewPlan(row.id)" 
                     class="clickable-row"></mat-row>
          </mat-table>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .plan-list-container {
      padding: 20px 0;
    }
    
    .actions-bar {
      display: flex;
      gap: 16px;
      margin-bottom: 20px;
    }
    
    .plans-card {
      width: 100%;
    }
    
    .loading-container {
      display: flex;
      justify-content: center;
      padding: 40px;
    }
    
    .clickable-row {
      cursor: pointer;
    }
    
    .clickable-row:hover {
      background-color: #f5f5f5;
    }
    
    .status-badge {
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
      font-weight: 500;
    }
    
    .status-draft {
      background-color: #e3f2fd;
      color: #1976d2;
    }
    
    .status-active {
      background-color: #e8f5e8;
      color: #388e3c;
    }
    
    .status-completed {
      background-color: #f3e5f5;
      color: #7b1fa2;
    }
  `]
})
export class PlanListComponent implements OnInit {
  plans: any[] = [];
  loading = false;
  displayedColumns = ['title', 'status', 'priority', 'created', 'actions'];

  constructor(
    private planningService: PlanningService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.loadPlans();
  }

  loadPlans() {
    this.loading = true;
    this.planningService.getPlans().subscribe({
      next: (plans) => {
        this.plans = plans;
        this.loading = false;
      },
      error: (error) => {
        this.snackBar.open('Error loading plans', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  createPlan() {
    this.router.navigate(['/planning/new']);
  }

  viewPlan(id: number) {
    this.router.navigate(['/planning', id]);
  }

  editPlan(id: number) {
    this.router.navigate(['/planning', id, 'edit']);
  }

  getAISuggestions() {
    this.snackBar.open('AI suggestions feature coming soon!', 'Close', { duration: 3000 });
  }
}
