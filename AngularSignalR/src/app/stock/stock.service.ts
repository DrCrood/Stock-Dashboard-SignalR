import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Stock } from './stock';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private stockUrl = environment.baseUrl + 'api/stock';

  constructor(private http: HttpClient) { }

  getStocks(): Observable<Stock[]> {
    return this.http.get<Stock[]>(this.stockUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  getStock(id: string): Observable<Stock> {
    const url = `${this.stockUrl}/${id}`;
    return this.http.get<Stock>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  addStock(stock: Stock): Observable<Stock> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<Stock>(this.stockUrl, stock, { headers: headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  removeStock(id: string): Observable<{}> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const url = `${this.stockUrl}/${id}`;
    return this.http.delete<Stock>(url, { headers: headers })
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