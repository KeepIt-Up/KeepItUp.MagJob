import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, input } from '@angular/core';
import { AlertComponent } from '../alert/alert.component';

export interface ErrorMessages {
  [statusCode: number]: string;
}

@Component({
  selector: 'app-error-alert',
  templateUrl: './error-alert.component.html',
  imports: [AlertComponent],
  standalone: true
})
export class ErrorAlertComponent {
  error = input.required<HttpErrorResponse | Error | undefined>();
  errorMessages = input<ErrorMessages>({
    0: 'The server is unavailable',
    400: 'Bad Request: The server cannot process the request due to invalid syntax.',
    401: 'Unauthorized: Authentication is required to access this resource.',
    403: 'Forbidden: You do not have permission to access this resource.',
    404: 'Not Found: The requested resource could not be found.',
    405: 'Method Not Allowed: The requested method is not supported for the URL.',
    408: 'Request Timeout: The server timed out waiting for the request.',
    409: 'Conflict: The request could not be completed due to a conflict with the current state of the resource.',
    500: 'Internal Server Error: Something went wrong on our servers.',
    502: 'Bad Gateway: The server received an invalid response from the upstream server.',
    503: 'Service Unavailable: The server is temporarily unable to handle the request.',
    504: 'Gateway Timeout: The server did not receive a timely response from the upstream server.'
  });

  httpErrorResponse = computed(() => this.error() instanceof HttpErrorResponse ? this.error() as HttpErrorResponse : undefined);
  applicationError = computed(() => this.error() instanceof Error ? this.error() as Error : undefined);
}
