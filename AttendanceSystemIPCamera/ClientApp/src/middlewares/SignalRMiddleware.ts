import * as signalR from "@microsoft/signalr";
import { MiddlewareAPI, Dispatch, Middleware } from "redux";
import { ApplicationState } from "../store";
import createSignalRConnection from './SignalRConnection';
import { sessionActionCreators } from '../store/session/actionCreators';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { ACTIONS as SESSION_ACTIONS } from '../store/session/actionCreators';

const ACTIONS = {
    ...SESSION_ACTIONS
};

let connection: signalR.HubConnection;

function createSignalRMiddleware() {
    const middleware: Middleware = ({ dispatch, getState }: MiddlewareAPI) => (
        next: Dispatch
    ) => action => {
        switch (action.type) {
            case ACTIONS.START_REAL_TIME_CONNECTION:
                /*
                connection = createSignalRConnection();
                if (connection) {
                    attachEvents(connection, dispatch);
                    connection.start();
                }
                */
                break;
        }
        return next(action);
    };

    return middleware;
}

export function attachEvents(connection: signalR.HubConnection, dispatch: any) {
    const interval = setInterval(() => {
        if (connection.state === signalR.HubConnectionState.Connected) {
            connection.send('heartbeat');
        }
    }, 10000);

    connection.onclose(() => {
        clearInterval(interval);
    });

    connection.on("attendeePresented", attendeeCode => {
        dispatch(sessionActionCreators.updateAttendeeRecordRealTime(attendeeCode));
    });

    connection.on("sessionEnded", sessionId => {
        // clearInterval(interval);
        // connection.stop();
        dispatch(sessionActionCreators.requestSession(sessionId));
        dispatch(sessionActionCreators.endTakingAttendance());
    });

    connection.on("keepAlive", () => {
    });

    connection.on("newChangeRequest", () => {
        console.log('receiving real time thingy here!');
        dispatch(changeRequestActionCreators.incrementUnresolvedCount());
    });
}

export default createSignalRMiddleware;