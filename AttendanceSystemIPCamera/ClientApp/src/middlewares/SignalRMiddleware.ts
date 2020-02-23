import * as signalR from "@microsoft/signalr";
import { MiddlewareAPI, Dispatch, Middleware } from "redux";
import { ApplicationState } from "../store";
import SignalRConnection from './SignalRConnection';
import { sessionActionCreators } from '../store/session/actionCreators';

function createSignalRMiddleware() {
    const middleware: Middleware = ({ getState }: MiddlewareAPI) => (
        next: Dispatch
    ) => action => {
        if (action.signalR) {
        }
        return next(action);
    };

    return middleware;
}

function attachSignalREvents(connection: signalR.HubConnection, store: any) {
    connection.on("attendeePresented", attendeeCode => {
        store.dispatch(sessionActionCreators.updateAttendeeRecordRealTime(attendeeCode));
    });

    connection.on("sessionEnded", sessionId => {
        store.dispatch(sessionActionCreators.requestSession(sessionId));
    });

    connection.on("keepAlive", () => {
        console.log('kept alive');
    });
}

export function signalRStart(store: any) {
    let connection = new SignalRConnection().connection;
    attachSignalREvents(connection, store);
    connection.on("disconnect", () => {
        setTimeout(() => {
            connection.start();
        }, 2000); // wait 2 seconds to reconnect to avoid doing this too frequently
    });
    connection.start();
}

export default createSignalRMiddleware;