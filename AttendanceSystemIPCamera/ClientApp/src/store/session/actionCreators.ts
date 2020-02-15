import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import { getActiveSession } from "../../services/session";

export const ACTIONS = {
    RECEIVE_ACTIVE_SESSION: 'RECEIVE_ACTIVE_SESSION'
}
function receiveActiveSession(activeSession: any) {
    return {
        type: ACTIONS.RECEIVE_ACTIVE_SESSION,
        activeSession
    };
}

export const requestActiveSession = (): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getActiveSession();
    dispatch(receiveActiveSession(apiResponse.data));
}

export const sessionActionCreator = {
    requestActiveSession
}
