import * as signalR from "@microsoft/signalr";
import { isChromium } from "../utils";

const createSignalRConnection = () => {
    let transport = {
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
    };
    if (isChromium()) {
        transport = {
            transport: signalR.HttpTransportType.LongPolling
        };
    }
    const conn = new signalR.HubConnectionBuilder()
        .withUrl("/hub", transport)
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Debug)
        .build();
    conn.serverTimeoutInMilliseconds = 100000; // 100 second
    return conn;
};

export default createSignalRConnection;