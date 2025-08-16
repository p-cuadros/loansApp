import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { LoanService, Loan } from './loan.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-loan-details',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatSnackBarModule],
  template: `
    <main class="loan-details">
      <button mat-button (click)="back()">Back</button>
      <h2>Loan Details</h2>

      <div *ngIf="!loan">Loading...</div>

      <div *ngIf="loan">
        <div *ngIf="!editing">
          <p><strong>Applicant:</strong> {{ loan.applicantName }}</p>
          <p><strong>Amount:</strong> {{ loan.amount | currency }}</p>
          <p><strong>Current balance:</strong> {{ loan.currentBalance | currency }}</p>
          <p><strong>Status:</strong> {{ loan.status }}</p>
          <button mat-button color="primary" (click)="startEdit()">Edit</button>
          <button mat-button (click)="loadHistory()">Refresh history</button>
        </div>

        <div *ngIf="editing">
          <label>Applicant <input [(ngModel)]="editModel.applicantName"/></label>
          <label>Amount <input type="number" [(ngModel)]="editModel.amount"/></label>
          <label>Current balance <input type="number" [(ngModel)]="editModel.currentBalance"/></label>
          <label>Status
            <select [(ngModel)]="editModel.status">
              <option value="active">active</option>
              <option value="paid">paid</option>
            </select>
          </label>
          <div>
            <button mat-button color="primary" (click)="save()">Save</button>
            <button mat-button (click)="cancel()">Cancel</button>
          </div>
        </div>

        <section class="history">
          <h3>History</h3>
          <div *ngIf="!history">No history loaded.</div>
          <ul *ngIf="history">
            <li *ngFor="let h of history">{{h.amount | currency}} — {{h.applicantName}} — {{h.currentBalance | currency}} — {{h.status}}</li>
          </ul>
        </section>
        <section class="payments">
          <h3>Payments</h3>
          <div *ngIf="!payments">No payments loaded.</div>
          <ul *ngIf="payments">
            <li *ngFor="let p of payments">{{p.amount | currency}} — {{p.datePayment | date:'short'}}</li>
          </ul>
        </section>
      </div>
    </main>
  `,
  styles: [`.loan-details { padding: 1rem; } .history { margin-top: 1rem; } label { display:block; margin: .5rem 0 }`]
})
export class LoanDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private loanService = inject(LoanService);
  private snack = inject(MatSnackBar);

  loan?: Loan;
  editing = false;
  editModel = { amount: 0, currentBalance: 0, applicantName: '', status: 'active' as 'active' | 'paid' };
  history: any[] | null = null;
  payments: any[] | null = null;

  ngOnInit(): void {
    this.load();
    this.loadHistory();
  this.loadPayments();
  }

  private id(): number {
    return Number(this.route.snapshot.paramMap.get('id')) || 0;
  }

  load() {
    const id = this.id();
    if (!id) { this.snack.open('Invalid loan id', 'OK', { duration: 2000 }); this.router.navigate(['/loans']); return; }
    this.loanService.get(id).subscribe({ next: (l) => { this.loan = l; }, error: () => this.snack.open('Failed to load loan', 'OK', { duration: 2000 }) });
  }

  loadHistory() {
    const id = this.id();
    if (!id) return;
    this.loanService.getHistory(id).subscribe({ next: (h) => this.history = h, error: () => this.snack.open('Failed to load history', 'OK', { duration: 2000 }) });
  }

  loadPayments() {
    const id = this.id();
    if (!id) return;
    this.loanService.getPayments(id).subscribe({ next: (p) => this.payments = p, error: () => this.snack.open('Failed to load payments', 'OK', { duration: 2000 }) });
  }

  back() { this.router.navigate(['/loans']); }

  startEdit() {
    if (!this.loan) return;
    this.editing = true;
    this.editModel = { amount: this.loan.amount, currentBalance: this.loan.currentBalance, applicantName: this.loan.applicantName, status: this.loan.status };
  }

  cancel() { this.editing = false; }

  save() {
    if (!this.loan) return;
    const payload = { id: this.loan.id, amount: Number(this.editModel.amount), currentBalance: Number(this.editModel.currentBalance), applicantName: (this.editModel.applicantName || '').trim(), status: this.editModel.status };
    this.loanService.update(this.loan.id, payload).subscribe({ next: (u) => { this.loan = u; this.editing = false; this.snack.open('Loan updated', 'OK', { duration: 2000 }); }, error: () => this.snack.open('Failed to update', 'OK', { duration: 2000 }) });
  }
}
