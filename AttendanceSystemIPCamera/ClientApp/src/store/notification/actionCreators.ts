import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import Notification from "../../models/Notification";

export const ACTIONS = {
    RECEIVE_NOTIFICATION: 'RECEIVE_NOTIFICATION'
};

function receiveNotifications(notification: Notification) {
    return {
        type: ACTIONS.RECEIVE_NOTIFICATION,
        notification
    };
}

export const notificationActionCreators = {
    receiveNotifications
};
