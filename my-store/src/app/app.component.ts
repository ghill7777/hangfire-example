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
    this.client.post("https://localhost:5001/api/v1/orders", order)
      .subscribe(data => {
        console.log(data);
        this.isBusy = false;
      });
  }
}
