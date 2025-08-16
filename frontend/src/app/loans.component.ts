import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CreateLoanRequest, Loan, LoanService } from './loan.service';
import { AuthService } from './auth.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatSnackBarModule, HttpClientModule, FormsModule],
  template: `
    <main class="loans">
      <section class="loans--container">
        <div class="toolbar">
          <button mat-stroked-button color="warn" (click)="logout()">Logout</button>
        </div>
        <h1>Loan Management</h1>
        <form (ngSubmit)="createLoan()" #loanForm="ngForm" class="create-loan-form">
          <label>
            Applicant
            <input name="applicantName" [(ngModel)]="form.applicantName" placeholder="Full name" required />
          </label>
          <label>
            Amount
            <input type="number" name="amount" [(ngModel)]="form.amount" min="0.01" step="0.01" required />
          </label>
          <button mat-raised-button color="accent" type="submit" [disabled]="creating">Create loan</button>
        </form>

  <table mat-table [dataSource]="loans" class="mat-elevation-z8">
          <ng-container matColumnDef="amount">
            <th mat-header-cell *matHeaderCellDef>Loan Amount</th>
            <td mat-cell *matCellDef="let element">{{ element.amount | currency }}</td>
          </ng-container>

          <ng-container matColumnDef="currentBalance">
            <th mat-header-cell *matHeaderCellDef>Current Balance</th>
            <td mat-cell *matCellDef="let element">{{ element.currentBalance | currency }}</td>
          </ng-container>

          <ng-container matColumnDef="applicantName">
            <th mat-header-cell *matHeaderCellDef>Applicant</th>
            <td mat-cell *matCellDef="let element">{{ element.applicantName }}</td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let element">{{ element.status }}</td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let element">
              <div *ngIf="!editing[element.id]">
                <button mat-button (click)="startEdit(element)">Edit</button>
                <button mat-button (click)="openDetails(element)">Details</button>
                <button mat-button (click)="toggleHistory(element)">History</button>
              </div>
              <div *ngIf="editing[element.id]" class="edit-row">
                <input type="text" [(ngModel)]="editModel[element.id].applicantName" />
                <input type="number" [(ngModel)]="editModel[element.id].amount" />
                <input type="number" [(ngModel)]="editModel[element.id].currentBalance" />
                <select [(ngModel)]="editModel[element.id].status">
                  <option value="active">active</option>
                  <option value="paid">paid</option>
                </select>
                <button mat-button color="primary" (click)="saveEdit(element)">Save</button>
                <button mat-button (click)="cancelEdit(element)">Cancel</button>
              </div>
              <div *ngIf="element.status === 'active' && !editing[element.id]" class="pay-action">
                <input type="number" placeholder="Amount" min="0.01" step="0.01" [(ngModel)]="paymentAmount[element.id]"/>
                <button mat-button color="primary" (click)="pay(element)" [disabled]="paying[element.id]">Pay</button>
              </div>
              <ng-template #paidBlock>
                <span>Paid</span>
              </ng-template>
              <div *ngIf="histories[element.id]">
                <h4>History</h4>
                <ul>
                  <li *ngFor="let h of histories[element.id]">{{h.amount | currency}} — {{h.applicantName}} — {{h.currentBalance | currency}} — {{h.status}}</li>
                </ul>
              </div>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns; sticky: true"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
        </table>
      </section>
    </main>
  `,
  styles: [`
    .create-loan-form { display:flex; gap: 1rem; align-items: end; margin: 1rem 0 2rem; }
    .create-loan-form label { display:flex; flex-direction: column; font-weight:500; }
    .pay-action { display:inline-flex; gap:.5rem; align-items:center; }
    .toolbar { display:flex; justify-content:flex-end; }
  `]
})
export class LoansComponent {
  private readonly loanService = inject(LoanService);
  private readonly auth = inject(AuthService);
  private readonly snack = inject(MatSnackBar);
  private readonly router = inject(Router);

  displayedColumns: string[] = ['amount', 'currentBalance', 'applicantName', 'status', 'actions'];
  loans: Loan[] = [];
  editing: Record<number, boolean> = {};
  editModel: Record<number, { amount: number; currentBalance: number; applicantName: string; status: 'active' | 'paid' }> = {};
  histories: Record<number, any[]> = {};
  form: CreateLoanRequest = { amount: 0, applicantName: '' };
  paymentAmount: Record<number, number> = {};
  creating = false;
  paying: Record<number, boolean> = {};

  constructor() {
    this.load();
  }

  openDetails(loan: Loan) {
    this.router.navigate(['/loans', loan.id]);
  }

  load() {
    this.loanService.list().subscribe((data) => (this.loans = data));
  }

  logout() { this.auth.logout(); this.router.navigate(['/login']); }

  createLoan() {
    const payload: CreateLoanRequest = {
      amount: Number(this.form.amount),
      applicantName: (this.form.applicantName || '').trim(),
    };
    if (!payload.applicantName || !(payload.amount > 0)) {
      this.snack.open('Please provide a valid applicant name and amount > 0', 'OK', { duration: 2500 });
      return;
    }
    this.creating = true;
    this.loanService.create(payload)
      .pipe(finalize(() => (this.creating = false)))
      .subscribe({
        next: () => { this.form = { amount: 0, applicantName: '' }; this.load(); this.snack.open('Loan created', 'OK', { duration: 2000 }); },
        error: (err) => this.snack.open(err?.message || 'Failed to create loan', 'Dismiss', { duration: 3500 })
      });
  }

  startEdit(loan: Loan) {
    this.editing[loan.id] = true;
    this.editModel[loan.id] = { amount: loan.amount, currentBalance: loan.currentBalance, applicantName: loan.applicantName, status: loan.status };
  }

  cancelEdit(loan: Loan) {
    this.editing[loan.id] = false;
    delete this.editModel[loan.id];
  }

  saveEdit(loan: Loan) {
    const model = this.editModel[loan.id];
    if (!model) return;
  const payload = { id: loan.id, amount: Number(model.amount), currentBalance: Number(model.currentBalance), applicantName: (model.applicantName || '').trim(), status: model.status as 'active' | 'paid' };
    if (!payload.applicantName || !(payload.amount >= 0)) { this.snack.open('Invalid data', 'OK', { duration: 2000 }); return; }
    this.loanService.update(loan.id, payload).subscribe({
      next: (updated) => {
        this.loans = this.loans.map(l => l.id === loan.id ? updated : l);
        this.editing[loan.id] = false;
        delete this.editModel[loan.id];
        this.snack.open('Loan updated', 'OK', { duration: 2000 });
      },
      error: (err) => this.snack.open(err?.message || 'Failed to update loan', 'Dismiss', { duration: 3500 })
    });
  }

  toggleHistory(loan: Loan) {
    if (this.histories[loan.id]) { delete this.histories[loan.id]; return; }
    this.loanService.getHistory(loan.id).subscribe({ next: (h) => this.histories[loan.id] = h, error: () => this.snack.open('Failed to load history', 'OK', { duration: 2000 }) });
  }

  pay(loan: Loan) {
    const amount = Number(this.paymentAmount[loan.id]);
    if (!(amount > 0)) { this.snack.open('Enter a valid payment amount', 'OK', { duration: 2000 }); return; }
    this.paying[loan.id] = true;
    this.loanService.makePayment(loan.id, { amount })
      .pipe(finalize(() => (this.paying[loan.id] = false)))
      .subscribe({
        next: (updated) => {
          // Replace immutably to trigger table refresh and status updates
          this.loans = this.loans.map(l => l.id === loan.id ? updated : l);
          this.paymentAmount[loan.id] = 0;
          this.snack.open('Payment applied', 'OK', { duration: 2000 });
        },
        error: (err) => this.snack.open(err?.message || 'Failed to apply payment', 'Dismiss', { duration: 3500 })
      });
  }
}
