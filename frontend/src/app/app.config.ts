import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { API_BASE_URL } from './tokens';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
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
