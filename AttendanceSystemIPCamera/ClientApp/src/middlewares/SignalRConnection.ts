import * as signalR from "@microsoft/signalr";

class SignalRConnection {
    private conn: signalR.HubConnection | undefined;
    get connection(): signalR.HubConnection {
        if (this.conn == null) {
            this.conn = new signalR.HubConnectionBuilder()
                .withUrl("/hub")
                .configureLogging(signalR.LogLevel.Information)
                .build();
            this.conn.serverTimeoutInMilliseconds = 100000; // 100 second
        }
        return this.conn;
    }
}

/*
const connectionSingleton = new SignalRConnection();
export default connectionSingleton.connection;
*/

export default SignalRConnection;