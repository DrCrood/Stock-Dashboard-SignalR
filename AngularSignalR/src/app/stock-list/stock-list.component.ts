import { Component, OnInit, ViewChild } from '@angular/core';
import { Stock } from '../stock/stock';
import { Notice} from '../notice/notice'
import { StockService } from '../stock/stock.service';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import {AgGridAngular} from 'ag-grid-angular';
import { GridOptions } from 'ag-grid-community';

@Component({
  selector: 'app-stock-list',
  templateUrl: './stock-list.component.html',
  styleUrls: ['./stock-list.component.css']
})
export class StockListComponent implements OnInit {
  pageTitle = 'Stock List';
  stocks: Stock[] = [];
  notices: Notice[] = [];
  symbols: string[] = [];
  errorMessage = '';
  api : any;
  columnApi : any;
  rowClassRules : any;

  @ViewChild('stockGrid') stockGrid: AgGridAngular;

  constructor(private stockService: StockService) { 
    this.rowClassRules = {
      'grid-row-green': function(params) { return params.data.change > 0; },
      'grid-row-red': function(params) { return params.data.change < 0; },
      'grid-row-blue': function(params) { return params.data.change ===  0; },
    }

  }

  columnDefs = [
    { field: 'symbol', sortable: true, filter: true, checkboxSelection: true, resizable: true },
    { field: 'price_pre', sortable: true, filter: true, resizable: true },
    { field: 'price_new', sortable: true, filter: true, resizable: true },
    { field: 'change', sortable: true, filter: true, resizable: true },
    { field: 'updateTime', sortable: true, filter: true, resizable: true }
  ];


  stockGridOptions : GridOptions = {
     
  };


  ngOnInit(): void {

    this.getStocks();

    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(environment.baseUrl + 'notify')
      .build();

    connection.start().then(function () {
      console.log('SignalR Connected!');
    }).catch(function (err) {
      return console.error(err.toString());
    });

    connection.on("PriceChangedEvent", () => {
      this.getStocks();
    });

    connection.on("ChangeNotice", (notice) => {
      if(this.symbols.includes(notice.stockSymbol))
      {
        let n = this.notices.find( s => s.stockSymbol === notice.stockSymbol);
        let i = this.notices.indexOf(n);
        this.notices[i] = notice;
      }
      else
      {
        this.notices.push(notice);
        this.symbols.push(notice.stockSymbol);
      }
    });
  }

  public onStockGridReady(event: any)
  {
    this.api = event.api;
    this.columnApi = event.columnApi;
    this.stockGridOptions.api.sizeColumnsToFit();

  }

  getStocks() {
    this.stockService.getStocks().subscribe(
      stocks => {
        this.stocks = stocks;
      },
      error => this.errorMessage = <any>error
    );
  }

  deleteStock(symbol: string): void {
      if (confirm(`Are you sure want to remove this Stock: ${symbol}?`)) {
        this.stockService.removeStock(symbol)
          .subscribe(
            () => this.onSaveComplete(),
            (error: any) => this.errorMessage = <any>error
          );
      }
  }

  onSaveComplete(): void {
    this.stockService.getStocks().subscribe(
      stocks => {
        this.stocks = stocks;
      },
      error => this.errorMessage = <any>error
    );
  }

}  