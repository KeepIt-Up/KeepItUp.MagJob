import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input } from '@angular/core';

export interface ErrorMessages {
  [statusCode: number]: string;
}

@Component({
  selector: 'app-http-error-response-alert',
  standalone: true,
  imports: [],
  templateUrl: './http-error-response-alert.component.html'
})
export class HttpErrorResponseAlertComponent {
  @Input({ required: true }) error!: HttpErrorResponse;
  @Input() showDetails = false;
  @Input() showIcon = true;
  @Input() errorMessages: ErrorMessages = {
    0: 'The back-end server is unavailable',
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
  };

  getErrorMessage(): string {
    if (this.showDetails) {
      return JSON.stringify(this.error);
    }

    return this.errorMessages[this.error.status] ||
      `Error ${this.error.status}`;
  }
}
