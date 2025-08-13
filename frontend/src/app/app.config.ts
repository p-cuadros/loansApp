import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { API_BASE_URL } from './tokens';
import { environment } from '../environments/environment';
import { httpErrorInterceptor } from './http-error.interceptor';
import { authInterceptor } from './auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
  provideHttpClient(withInterceptors([authInterceptor, httpErrorInterceptor])),
    {
      provide: API_BASE_URL,
      useFactory: () => {
        if (typeof window !== 'undefined') {
          const loc = window.location;
          if (loc.port === '4200') {
            return `${loc.protocol}//${loc.hostname}:8080`;
          }
        }
        return environment.apiBaseUrl;
      },
    },
  ]
};
