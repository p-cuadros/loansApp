import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

export const CORRELATION_ID_HEADER = 'X-Correlation-Id';

export const httpErrorInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  // Use Web Crypto API to generate a correlation id per request
  const correlationId = (globalThis.crypto?.randomUUID?.() ?? Math.random().toString(36).slice(2));
  const request = req.clone({ setHeaders: { [CORRELATION_ID_HEADER]: correlationId } });

  const startedAt = Date.now();
  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const durationMs = Date.now() - startedAt;
      const payload = {
        level: 'error',
        type: 'http_error',
        method: request.method,
        url: request.urlWithParams,
        status: error.status,
        error: (error as any)?.error?.error ?? 'unknown_error',
        message: (error as any)?.error?.message ?? error.message,
        correlationId: (error as any)?.error?.correlationId ?? correlationId,
        durationMs
      };
      // Structured console log
      console.error(JSON.stringify(payload));

      const friendly = (error as any)?.error?.message || 'Request failed. Please try again.';
      return throwError(() => new Error(friendly));
    })
  );
};
