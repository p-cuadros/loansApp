import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE_URL } from './tokens';
import { Observable } from 'rxjs';

export interface Loan {
  id: number;
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: 'active' | 'paid';
}

export interface CreateLoanRequest {
  amount: number;
  applicantName: string;
}

export interface PaymentRequest {
  amount: number;
}

export interface EditLoanRequest {
  id: number;
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: 'active' | 'paid';
}

export interface LoanHistoryItem {
  id: number;
  idLoan: number;
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: string;
}

export interface PaymentItem {
  idPayment: number;
  idLoan: number;
  datePayment: string;
  amount: number;
}

@Injectable({ providedIn: 'root' })
export class LoanService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  list(): Observable<Loan[]> {
    return this.http.get<Loan[]>(`${this.baseUrl}/loans`);
  }

  create(data: CreateLoanRequest): Observable<Loan> {
    return this.http.post<Loan>(`${this.baseUrl}/loans`, data);
  }

  makePayment(loanId: number, data: PaymentRequest): Observable<Loan> {
    return this.http.post<Loan>(`${this.baseUrl}/loans/${loanId}/payment`, data);
  }

  update(loanId: number, data: EditLoanRequest): Observable<Loan> {
    return this.http.put<Loan>(`${this.baseUrl}/loans/${loanId}`, data);
  }

  get(id: number): Observable<Loan> {
    return this.http.get<Loan>(`${this.baseUrl}/loans/${id}`);
  }

  getHistory(loanId: number): Observable<LoanHistoryItem[]> {
  return this.http.get<LoanHistoryItem[]>(`${this.baseUrl}/loanhistory/${loanId}`);
  }

  getPayments(loanId: number): Observable<PaymentItem[]> {
    return this.http.get<PaymentItem[]>(`${this.baseUrl}/loans/${loanId}/payments`);
  }
}
