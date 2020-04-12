import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import Notification from "../../models/Notification";

export const ACTIONS = {
    RECEIVE_NOTIFICATION: 'RECEIVE_NOTIFICATION',
    MARK_AS_READ: 'MARK_AS_READ',
    MARK_ALL_AS_READ: 'MARK_ALL_AS_READ'
};

function receiveNotification(notification: Notification) {
    return {
        type: ACTIONS.RECEIVE_NOTIFICATION,
        notification
    };
}

function markAsRead(id: string) {
    return {
        type: ACTIONS.MARK_AS_READ,
        id
    };
}

function markAllAsRead() {
    return {
        type: ACTIONS.MARK_ALL_AS_READ,
    };
}

export const notificationActionCreators = {
    receiveNotification,
    markAsRead,
    markAllAsRead
};
