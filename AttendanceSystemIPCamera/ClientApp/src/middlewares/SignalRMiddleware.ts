import * as signalR from "@microsoft/signalr";
import { MiddlewareAPI, Dispatch, Middleware } from "redux";
import { ApplicationState } from "../store";
import signalRConnection from './SignalRConnection';
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

export function signalRStart(store: any) {
    let connection = signalRConnection;

    connection.on("attendeePresented", attendeeCode => {
        store.dispatch(sessionActionCreators.updateAttendeeRecordRealTime(attendeeCode));
    });

    connection.start();
}

export default createSignalRMiddleware;