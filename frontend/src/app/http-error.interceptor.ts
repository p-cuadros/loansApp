import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, catchError, finalize, throwError, tap } from 'rxjs';

export const CORRELATION_ID_HEADER = 'X-Correlation-Id';

export const httpErrorInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  // Use Web Crypto API to generate a correlation id per request
  const correlationId = (globalThis.crypto?.randomUUID?.() ?? Math.random().toString(36).slice(2));
  const request = req.clone({ setHeaders: { [CORRELATION_ID_HEADER]: correlationId } });

  const startedAt = Date.now();
  let statusCode = 0;
  return next(request).pipe(
    tap((event: HttpEvent<unknown>) => {
      if (event instanceof HttpResponse) {
        statusCode = event.status;
      }
    }),
    catchError((error: HttpErrorResponse) => {
      const durationMs = Date.now() - startedAt;
      statusCode = error.status;
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
      console.error(JSON.stringify(payload));
      const friendly = (error as any)?.error?.message || 'Request failed. Please try again.';
      return throwError(() => new Error(friendly));
    }),
    finalize(() => {
      // fire-and-forget metric to backend
      try {
        const loc = window.location;
        const base = loc.port === '4200' ? `${loc.protocol}//${loc.hostname}:8080` : `${loc.protocol}//${loc.host}`;
        fetch(`${base}/metrics/http-client`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            method: request.method,
            url: request.url,
            status: statusCode,
            durationMs: Date.now() - startedAt,
            correlationId
          })
        }).catch(() => {});
      } catch (e) {
        // ignore client metric failure
      }
    })
  );
};
