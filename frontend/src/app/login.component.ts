import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatSnackBarModule],
  template: `
    <main class="login">
      <section class="login--container">
        <h1>Login</h1>
        <form (ngSubmit)="login()" class="login-form">
          <label>
            Username
            <input name="username" [(ngModel)]="model.username" placeholder="username" />
          </label>
          <label>
            Password
            <input type="password" name="password" [(ngModel)]="model.password" placeholder="password" />
          </label>
          <button mat-raised-button color="primary" type="submit" [disabled]="loading">Login</button>
        </form>
      </section>
    </main>
  `,
  styles: [`
    .login { display:flex; justify-content:center; padding: 2rem; }
    .login--container { width: 100%; max-width: 480px; }
    .login-form { display:flex; gap:1rem; flex-direction: column; }
    .login-form label { display:flex; flex-direction:column; font-weight:500; }
  `]
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly snack = inject(MatSnackBar);

  model = { username: 'admin', password: 'admin' };
  loading = false;

  async login() {
    this.loading = true;
    try {
      const res = await this.auth.login(this.model.username, this.model.password);
      if (res?.token) {
        this.snack.open('Login exitoso', 'OK', { duration: 2000 });
        this.router.navigate(['/loans']);
      }
    } catch (e: any) {
      this.snack.open(e?.message || 'Login fallido', 'Cerrar', { duration: 3500 });
    } finally {
      this.loading = false;
    }
  }
}
