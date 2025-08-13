import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const token = localStorage.getItem('token');
  if (token) {
    const withAuth = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
    return next(withAuth);
  }
  return next(req);
};
