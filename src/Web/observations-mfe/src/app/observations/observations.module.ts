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
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { ObservationsComponent } from './observations.component';
import { ObservationListComponent } from './components/observation-list/observation-list.component';
import { ObservationFormComponent } from './components/observation-form/observation-form.component';
import { ObservationDetailComponent } from './components/observation-detail/observation-detail.component';
import { DataVisualizationComponent } from './components/data-visualization/data-visualization.component';
import { ObservationsService } from './services/observations.service';

const routes = [
  {
    path: '',
    component: ObservationsComponent,
    children: [
      { path: '', component: ObservationListComponent },
      { path: 'new', component: ObservationFormComponent },
      { path: 'analytics', component: DataVisualizationComponent },
      { path: ':id', component: ObservationDetailComponent },
      { path: ':id/edit', component: ObservationFormComponent }
    ]
  }
];

@NgModule({
  declarations: [
    ObservationsComponent,
    ObservationListComponent,
    ObservationFormComponent,
    ObservationDetailComponent,
    DataVisualizationComponent
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
    MatChipsModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  providers: [
    ObservationsService
  ]
})
export class ObservationsModule { }
