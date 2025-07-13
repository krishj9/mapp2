import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PlanningService } from '../../services/planning.service';

@Component({
  selector: 'app-plan-form',
  template: `
    <div class="plan-form-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>
            {{isEdit ? 'Edit Plan' : 'Create New Plan'}}
          </mat-card-title>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="planForm" (ngSubmit)="onSubmit()">
            <div class="form-row">
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Title</mat-label>
                <input matInput formControlName="title" placeholder="Enter plan title">
                <mat-error *ngIf="planForm.get('title')?.hasError('required')">
                  Title is required
                </mat-error>
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Description</mat-label>
                <textarea matInput formControlName="description" 
                         placeholder="Enter plan description" rows="4"></textarea>
              </mat-form-field>
            </div>

            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Priority</mat-label>
                <mat-select formControlName="priority">
                  <mat-option value="1">Low</mat-option>
                  <mat-option value="2">Medium</mat-option>
                  <mat-option value="3">High</mat-option>
                  <mat-option value="4">Critical</mat-option>
                </mat-select>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Status</mat-label>
                <mat-select formControlName="status">
                  <mat-option value="0">Draft</mat-option>
                  <mat-option value="1">Active</mat-option>
                  <mat-option value="2">Completed</mat-option>
                  <mat-option value="3">Cancelled</mat-option>
                </mat-select>
              </mat-form-field>
            </div>

            <div class="ai-section" *ngIf="!isEdit">
              <button type="button" mat-stroked-button color="accent" 
                      (click)="getAISuggestions()" [disabled]="!planForm.get('title')?.value">
                <mat-icon>psychology</mat-icon>
                Get AI Suggestions
              </button>
              
              <div *ngIf="aiSuggestions.length > 0" class="ai-suggestions">
                <h4>AI Suggestions:</h4>
                <ul>
                  <li *ngFor="let suggestion of aiSuggestions">{{suggestion}}</li>
                </ul>
              </div>
            </div>
          </form>
        </mat-card-content>

        <mat-card-actions align="end">
          <button mat-button type="button" (click)="cancel()">Cancel</button>
          <button mat-raised-button color="primary" 
                  (click)="onSubmit()" 
                  [disabled]="planForm.invalid || saving">
            <mat-spinner diameter="20" *ngIf="saving"></mat-spinner>
            {{saving ? 'Saving...' : (isEdit ? 'Update' : 'Create')}}
          </button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .plan-form-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px 0;
    }
    
    .form-row {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
    }
    
    .full-width {
      width: 100%;
    }
    
    .ai-section {
      margin: 20px 0;
      padding: 16px;
      border: 1px solid #e0e0e0;
      border-radius: 4px;
      background-color: #fafafa;
    }
    
    .ai-suggestions {
      margin-top: 16px;
    }
    
    .ai-suggestions h4 {
      margin: 0 0 8px 0;
      color: #1976d2;
    }
    
    .ai-suggestions ul {
      margin: 0;
      padding-left: 20px;
    }
    
    .ai-suggestions li {
      margin-bottom: 4px;
    }
  `]
})
export class PlanFormComponent implements OnInit {
  planForm: FormGroup;
  isEdit = false;
  planId: number | null = null;
  saving = false;
  aiSuggestions: string[] = [];

  constructor(
    private fb: FormBuilder,
    private planningService: PlanningService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.planForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      priority: [2], // Medium
      status: [0] // Draft
    });
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEdit = true;
        this.planId = +params['id'];
        this.loadPlan();
      }
    });
  }

  loadPlan() {
    if (this.planId) {
      this.planningService.getPlan(this.planId).subscribe({
        next: (plan) => {
          this.planForm.patchValue(plan);
        },
        error: (error) => {
          this.snackBar.open('Error loading plan', 'Close', { duration: 3000 });
        }
      });
    }
  }

  onSubmit() {
    if (this.planForm.valid) {
      this.saving = true;
      const planData = this.planForm.value;

      const operation = this.isEdit 
        ? this.planningService.updatePlan(this.planId!, planData)
        : this.planningService.createPlan(planData);

      operation.subscribe({
        next: (result) => {
          this.saving = false;
          this.snackBar.open(
            this.isEdit ? 'Plan updated successfully' : 'Plan created successfully',
            'Close',
            { duration: 3000 }
          );
          this.router.navigate(['/planning']);
        },
        error: (error) => {
          this.saving = false;
          this.snackBar.open('Error saving plan', 'Close', { duration: 3000 });
        }
      });
    }
  }

  getAISuggestions() {
    const title = this.planForm.get('title')?.value;
    if (title) {
      this.planningService.getAISuggestions(title).subscribe({
        next: (suggestions) => {
          this.aiSuggestions = suggestions;
        },
        error: (error) => {
          this.snackBar.open('Error getting AI suggestions', 'Close', { duration: 3000 });
        }
      });
    }
  }

  cancel() {
    this.router.navigate(['/planning']);
  }
}
