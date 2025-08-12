import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { HttpClientModule } from '@angular/common/http';
import { Loan, LoanService } from './loan.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  private readonly loanService = inject(LoanService);
  displayedColumns: string[] = ['amount', 'currentBalance', 'applicantName', 'status'];
  loans: Loan[] = [];

  constructor() {
    this.loanService.list().subscribe((data) => (this.loans = data));
  }
}
