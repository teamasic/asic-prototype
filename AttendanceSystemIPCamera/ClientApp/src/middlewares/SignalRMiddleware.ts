import * as signalR from "@microsoft/signalr";
import { MiddlewareAPI, Dispatch, Middleware } from "redux";
import { ApplicationState } from "../store";
import createSignalRConnection from './SignalRConnection';
import { sessionActionCreators } from '../store/session/actionCreators';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { ACTIONS as SESSION_ACTIONS } from '../store/session/actionCreators';
import { success } from '../utils';

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
            // console.log('heartbeat');
        }
    }, 10000);

    connection.onclose(() => {
        clearInterval(interval);
    });

    connection.on("attendeePresented", attendeeCode => {
        dispatch(sessionActionCreators.updateAttendeeRecordRealTime(attendeeCode));
    });

    connection.on("attendeeUnknown", image => {
        dispatch(sessionActionCreators.updateUnknownRealTime(image));
    });

    connection.on("attendeePresentedBatch", (attendeeCodeString: string) => {
        const codes = attendeeCodeString.split(",");
        dispatch(sessionActionCreators.updateAttendeeRecordRealTimeBatch(codes));
    });

    connection.on("attendeeUnknownBatch", (imageString: string) => {
        const images = imageString.split(",");
        dispatch(sessionActionCreators.updateUnknownRealTimeBatch(images));
    });

    connection.on("sessionEnded", sessionId => {
        // clearInterval(interval);
        // connection.stop();
        success("The taking attendance process has finished.")
        dispatch(sessionActionCreators.requestSession(sessionId));
        dispatch(sessionActionCreators.endTakingAttendance());
    });

    connection.on("keepAlive", () => {
        // console.log('keep alive');
    });

    connection.on("newChangeRequest", () => {
        console.log('receiving real time thingy here!');
        dispatch(changeRequestActionCreators.incrementUnresolvedCount());
    });
}

export default createSignalRMiddleware;