import { Component } from '@angular/core';
import { HttpClient } from "@angular/common/http";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'my-store';
  isBusy = false;
  orderPlaced = false;
  orderNumber = '';
  isError = false;

  /**
   *
   */
  constructor(private client: HttpClient) {

  }

  async orderClick(): Promise<void> {
    const order = {
      customerName: "Greg",
      email: "ghill@keefegroup.com",
      items: "batteries,toothpaste,soap"
    };
    this.isBusy = true;
    this.orderPlaced = false;
    this.orderNumber = "";
    this.isError = false;
    this.client.post("https://localhost:5001/api/v1/orders", order)
      .subscribe((data: any) => {
        console.log(data);
        this.isBusy = false;
        this.orderPlaced = true;
        this.orderNumber = data.id.toString();
        this.isError = false;
      }, err => {
        this.isError = true;
        this.isBusy = false;
        this.orderPlaced = false;
      });
  }
}
