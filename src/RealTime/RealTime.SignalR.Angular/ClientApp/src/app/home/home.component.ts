import {Component, OnInit} from '@angular/core';
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public hubConnection: HubConnection;
  public messages: string[] = [];
  public message: string;
  public user: string;

  constructor() {
  }


  ngOnInit() {
    // SIGNALR MESSAGE HUB
    let builder = new HubConnectionBuilder();
    this.hubConnection = builder.withUrl('/chathub').build();
    this.hubConnection.on('ReceiveMessage', (user, message) => {
      this.messages.push('user: ' + user + "  Message: " + message);
      console.log(message);
    });
    this.hubConnection.start();
  }

  // signalr, send msg from client
  sendMessageToServer() {
    this.hubConnection.invoke('SendMessage', this.user, this.message);
  }
}
