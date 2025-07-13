import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PlanningService } from '../../services/planning.service';

@Component({
  selector: 'app-plan-detail',
  template: `
    <div class="plan-detail-container" *ngIf="plan">
      <div class="header-actions">
        <button mat-button (click)="goBack()">
          <mat-icon>arrow_back</mat-icon>
          Back to Plans
        </button>
        <div class="actions">
          <button mat-button (click)="editPlan()">
            <mat-icon>edit</mat-icon>
            Edit
          </button>
          <button mat-button color="accent" (click)="getAIAnalytics()">
            <mat-icon>psychology</mat-icon>
            AI Analytics
          </button>
        </div>
      </div>

      <mat-card class="plan-info-card">
        <mat-card-header>
          <mat-card-title>{{plan.title}}</mat-card-title>
          <mat-card-subtitle>
            <span class="status-badge" [class]="'status-' + getStatusName(plan.status)">
              {{getStatusName(plan.status)}}
            </span>
            <span class="priority-badge" [class]="'priority-' + getPriorityName(plan.priority)">
              {{getPriorityName(plan.priority)}} Priority
            </span>
          </mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <div class="plan-details">
            <div class="detail-section">
              <h3>Description</h3>
              <p>{{plan.description || 'No description provided'}}</p>
            </div>

            <div class="detail-section">
              <h3>Plan Information</h3>
              <div class="info-grid">
                <div class="info-item">
                  <strong>Created:</strong> {{plan.created | date:'medium'}}
                </div>
                <div class="info-item">
                  <strong>Last Modified:</strong> {{plan.lastModified | date:'medium'}}
                </div>
                <div class="info-item">
                  <strong>Items Count:</strong> {{plan.itemsCount || 0}}
                </div>
              </div>
            </div>

            <div class="detail-section" *ngIf="plan.items && plan.items.length > 0">
              <h3>Plan Items</h3>
              <mat-table [dataSource]="plan.items">
                <ng-container matColumnDef="title">
                  <mat-header-cell *matHeaderCellDef>Title</mat-header-cell>
                  <mat-cell *matCellDef="let item">{{item.title}}</mat-cell>
                </ng-container>

                <ng-container matColumnDef="status">
                  <mat-header-cell *matHeaderCellDef>Status</mat-header-cell>
                  <mat-cell *matCellDef="let item">{{item.status}}</mat-cell>
                </ng-container>

                <ng-container matColumnDef="dueDate">
                  <mat-header-cell *matHeaderCellDef>Due Date</mat-header-cell>
                  <mat-cell *matCellDef="let item">{{item.dueDate | date:'short'}}</mat-cell>
                </ng-container>

                <mat-header-row *matHeaderRowDef="itemColumns"></mat-header-row>
                <mat-row *matRowDef="let row; columns: itemColumns;"></mat-row>
              </mat-table>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="ai-analytics-card" *ngIf="aiAnalytics">
        <mat-card-header>
          <mat-card-title>
            <mat-icon>psychology</mat-icon>
            AI Analytics
          </mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="analytics-grid">
            <div class="metric">
              <h4>Completion Probability</h4>
              <div class="metric-value">{{aiAnalytics.completion_probability * 100 | number:'1.0-0'}}%</div>
            </div>
            <div class="metric">
              <h4>Risk Factors</h4>
              <ul>
                <li *ngFor="let risk of aiAnalytics.risk_factors">{{risk}}</li>
              </ul>
            </div>
            <div class="metric">
              <h4>Success Indicators</h4>
              <ul>
                <li *ngFor="let indicator of aiAnalytics.success_indicators">{{indicator}}</li>
              </ul>
            </div>
            <div class="metric">
              <h4>Recommendations</h4>
              <ul>
                <li *ngFor="let rec of aiAnalytics.recommendations">{{rec}}</li>
              </ul>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <div class="loading-container" *ngIf="loading">
      <mat-spinner></mat-spinner>
    </div>
  `,
  styles: [`
    .plan-detail-container {
      max-width: 1000px;
      margin: 0 auto;
      padding: 20px 0;
    }
    
    .header-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    
    .actions {
      display: flex;
      gap: 8px;
    }
    
    .plan-info-card, .ai-analytics-card {
      margin-bottom: 20px;
    }
    
    .status-badge, .priority-badge {
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
      font-weight: 500;
      margin-right: 8px;
    }
    
    .status-draft { background-color: #e3f2fd; color: #1976d2; }
    .status-active { background-color: #e8f5e8; color: #388e3c; }
    .status-completed { background-color: #f3e5f5; color: #7b1fa2; }
    
    .priority-low { background-color: #f1f8e9; color: #689f38; }
    .priority-medium { background-color: #fff3e0; color: #f57c00; }
    .priority-high { background-color: #fce4ec; color: #c2185b; }
    .priority-critical { background-color: #ffebee; color: #d32f2f; }
    
    .detail-section {
      margin-bottom: 24px;
    }
    
    .detail-section h3 {
      margin: 0 0 12px 0;
      color: #1976d2;
    }
    
    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
    }
    
    .info-item {
      padding: 8px 0;
    }
    
    .analytics-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 20px;
    }
    
    .metric h4 {
      margin: 0 0 8px 0;
      color: #1976d2;
    }
    
    .metric-value {
      font-size: 24px;
      font-weight: bold;
      color: #388e3c;
    }
    
    .metric ul {
      margin: 0;
      padding-left: 16px;
    }
    
    .loading-container {
      display: flex;
      justify-content: center;
      padding: 40px;
    }
  `]
})
export class PlanDetailComponent implements OnInit {
  plan: any = null;
  aiAnalytics: any = null;
  loading = false;
  itemColumns = ['title', 'status', 'dueDate'];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private planningService: PlanningService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      this.loadPlan(id);
    });
  }

  loadPlan(id: number) {
    this.loading = true;
    this.planningService.getPlan(id).subscribe({
      next: (plan) => {
        this.plan = plan;
        this.loading = false;
      },
      error: (error) => {
        this.snackBar.open('Error loading plan', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  editPlan() {
    this.router.navigate(['/planning', this.plan.id, 'edit']);
  }

  goBack() {
    this.router.navigate(['/planning']);
  }

  getAIAnalytics() {
    this.planningService.getAIAnalytics(this.plan.id).subscribe({
      next: (analytics) => {
        this.aiAnalytics = analytics;
      },
      error: (error) => {
        this.snackBar.open('Error loading AI analytics', 'Close', { duration: 3000 });
      }
    });
  }

  getStatusName(status: number): string {
    const statuses = ['draft', 'active', 'completed', 'cancelled'];
    return statuses[status] || 'unknown';
  }

  getPriorityName(priority: number): string {
    const priorities = ['', 'low', 'medium', 'high', 'critical'];
    return priorities[priority] || 'unknown';
  }
}
