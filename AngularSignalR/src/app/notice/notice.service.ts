import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Notice } from './notice';

@Injectable({
  providedIn: 'root'
})
export class NoticeService {

  private noticeUrl = environment.baseUrl +'api';

  constructor(private http: HttpClient) { }

  getNoticeALL(): Observable<Array<Notice>> {
    const url = `${this.noticeUrl}/notice/all`;
    return this.http.get<Array<Notice>>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  getNoticeCount(): Observable<number> {
    const url = `${this.noticeUrl}/notice/count`;
    return this.http.get<number>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  getNotice(id: string): Observable<Array<Notice>> {
    const url = `${this.noticeUrl}/notice/{id}`;
    return this.http.get<Array<Notice>>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(err) {
    let errorMessage: string;
    if (err.error instanceof ErrorEvent) {
      errorMessage = `An error occurred: ${err.error.message}`;
    } else {
      errorMessage = `Backend returned code ${err.status}: ${err.body.error}`;
    }
    console.error(err);
    return throwError(errorMessage);
  }
}
