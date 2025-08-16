import { Routes, CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { LoginComponent } from './login.component';
import { LoansComponent } from './loans.component';
import { LoanDetailsComponent } from './loan-details.component';

// Minimal inline canActivate guard
const authGuard: CanActivateFn = () => {
	const auth = inject(AuthService);
	const router = inject(Router);
	return auth.isLoggedIn ? true : router.createUrlTree(['/login']);
};

export const routes: Routes = [
	{ path: '', pathMatch: 'full', redirectTo: 'loans' },
	{ path: 'login', component: LoginComponent },
	{ path: 'loans', component: LoansComponent, canActivate: [authGuard] },
	{ path: 'loans/:id', component: LoanDetailsComponent, canActivate: [authGuard] },
	{ path: '**', redirectTo: 'loans' }
];
