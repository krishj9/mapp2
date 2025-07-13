import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

// Angular Material
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { PlanningComponent } from './planning.component';
import { PlanListComponent } from './components/plan-list/plan-list.component';
import { PlanFormComponent } from './components/plan-form/plan-form.component';
import { PlanDetailComponent } from './components/plan-detail/plan-detail.component';
import { PlanningService } from './services/planning.service';

const routes = [
  {
    path: '',
    component: PlanningComponent,
    children: [
      { path: '', component: PlanListComponent },
      { path: 'new', component: PlanFormComponent },
      { path: ':id', component: PlanDetailComponent },
      { path: ':id/edit', component: PlanFormComponent }
    ]
  }
];

@NgModule({
  declarations: [
    PlanningComponent,
    PlanListComponent,
    PlanFormComponent,
    PlanDetailComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule.forChild(routes),
    
    // Angular Material
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  providers: [
    PlanningService
  ]
})
export class PlanningModule { }
