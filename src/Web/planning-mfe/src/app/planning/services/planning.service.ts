import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PlanningService {
  private apiUrl = environment.planningApiUrl || 'http://localhost:5001/api';
  private aiApiUrl = environment.planningAiApiUrl || 'http://localhost:8001/api';

  constructor(private http: HttpClient) {}

  getPlans(): Observable<any[]> {
    // Mock data for now - replace with actual API call
    return of([
      {
        id: 1,
        title: 'Q1 Marketing Campaign',
        description: 'Launch new product marketing campaign',
        status: 1, // Active
        priority: 3, // High
        created: new Date('2024-01-15'),
        lastModified: new Date('2024-01-20'),
        itemsCount: 5
      },
      {
        id: 2,
        title: 'Website Redesign',
        description: 'Complete overhaul of company website',
        status: 0, // Draft
        priority: 2, // Medium
        created: new Date('2024-01-10'),
        lastModified: new Date('2024-01-18'),
        itemsCount: 8
      },
      {
        id: 3,
        title: 'Team Training Program',
        description: 'Implement comprehensive training for new hires',
        status: 2, // Completed
        priority: 2, // Medium
        created: new Date('2024-01-05'),
        lastModified: new Date('2024-01-25'),
        itemsCount: 3
      }
    ]);
    
    // Actual API call would be:
    // return this.http.get<any[]>(`${this.apiUrl}/plans`);
  }

  getPlan(id: number): Observable<any> {
    // Mock data for now - replace with actual API call
    return of({
      id: id,
      title: 'Q1 Marketing Campaign',
      description: 'Launch new product marketing campaign for Q1 2024',
      status: 1, // Active
      priority: 3, // High
      created: new Date('2024-01-15'),
      lastModified: new Date('2024-01-20'),
      itemsCount: 5,
      items: [
        {
          id: 1,
          title: 'Market Research',
          status: 'Completed',
          dueDate: new Date('2024-02-01')
        },
        {
          id: 2,
          title: 'Creative Development',
          status: 'In Progress',
          dueDate: new Date('2024-02-15')
        },
        {
          id: 3,
          title: 'Campaign Launch',
          status: 'Pending',
          dueDate: new Date('2024-03-01')
        }
      ]
    });
    
    // Actual API call would be:
    // return this.http.get<any>(`${this.apiUrl}/plans/${id}`);
  }

  createPlan(plan: any): Observable<any> {
    // Mock response - replace with actual API call
    return of({ id: Math.floor(Math.random() * 1000), ...plan });
    
    // Actual API call would be:
    // return this.http.post<any>(`${this.apiUrl}/plans`, plan);
  }

  updatePlan(id: number, plan: any): Observable<any> {
    // Mock response - replace with actual API call
    return of({ id, ...plan });
    
    // Actual API call would be:
    // return this.http.put<any>(`${this.apiUrl}/plans/${id}`, plan);
  }

  deletePlan(id: number): Observable<any> {
    // Actual API call would be:
    return this.http.delete(`${this.apiUrl}/plans/${id}`);
  }

  getAISuggestions(title: string, description?: string): Observable<string[]> {
    // Mock AI suggestions - replace with actual AI API call
    return of([
      `Break down '${title}' into smaller, manageable tasks`,
      'Set clear milestones and deadlines',
      'Identify potential risks and mitigation strategies',
      'Allocate resources and assign responsibilities'
    ]);
    
    // Actual AI API call would be:
    // return this.http.post<any>(`${this.aiApiUrl}/suggestions`, { title, description })
    //   .pipe(map(response => response.suggestions));
  }

  getAIAnalytics(planId: number): Observable<any> {
    // Mock AI analytics - replace with actual AI API call
    return of({
      plan_id: planId,
      completion_probability: 0.78,
      risk_factors: ['Resource constraints', 'Timeline pressure'],
      success_indicators: ['Clear milestones', 'Engaged team'],
      recommendations: [
        'Add buffer time for critical tasks',
        'Increase communication frequency'
      ]
    });
    
    // Actual AI API call would be:
    // return this.http.get<any>(`${this.aiApiUrl}/analytics/${planId}`);
  }

  optimizePlan(planId: number, items: string[], constraints?: string[]): Observable<any> {
    // Actual AI API call would be:
    return this.http.post<any>(`${this.aiApiUrl}/optimize`, {
      plan_id: planId,
      current_items: items,
      constraints: constraints
    });
  }
}
