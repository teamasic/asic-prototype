﻿import * as signalR from "@microsoft/signalr";

const createSignalRConnection = () => {
    const conn = new signalR.HubConnectionBuilder()
        .withUrl("/hub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    conn.serverTimeoutInMilliseconds = 100000; // 100 second
    return conn;
};

export default createSignalRConnection;