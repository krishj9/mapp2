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
import { MatTabsModule } from '@angular/material/tabs';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { ReportsComponent } from './reports.component';
import { ReportListComponent } from './components/report-list/report-list.component';
import { ReportBuilderComponent } from './components/report-builder/report-builder.component';
import { ReportViewerComponent } from './components/report-viewer/report-viewer.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ReportsService } from './services/reports.service';

const routes = [
  {
    path: '',
    component: ReportsComponent,
    children: [
      { path: '', component: DashboardComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'list', component: ReportListComponent },
      { path: 'builder', component: ReportBuilderComponent },
      { path: 'viewer/:id', component: ReportViewerComponent }
    ]
  }
];

@NgModule({
  declarations: [
    ReportsComponent,
    ReportListComponent,
    ReportBuilderComponent,
    ReportViewerComponent,
    DashboardComponent
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
    MatProgressSpinnerModule,
    MatTabsModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  providers: [
    ReportsService
  ]
})
export class ReportsModule { }
