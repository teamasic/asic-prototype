import Group from "../../models/Group";
import ApiResponse from "../../models/ApiResponse";
import { getGroups } from "../../services/group";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";

export const ACTIONS = {
    START_REQUEST_GROUPS: 'START_REQUEST_GROUPS',
    STOP_REQUEST_GROUPS_WITH_ERRORS: 'STOP_REQUEST_GROUPS_WITH_ERRORS',
    RECEIVE_GROUPS_DATA: 'RECEIVE_GROUPS_DATA'
}

function startRequestGroups() {
    return {
        type: ACTIONS.START_REQUEST_GROUPS
    };
}

function stopRequestGroupsWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_GROUPS_WITH_ERRORS,
        errors
    };
}

function receiveGroupsData(groups: Group[]) {
    return {
        type: ACTIONS.RECEIVE_GROUPS_DATA,
        groups
    };
}

const requestGroups = (): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestGroups());

    const apiResponse: ApiResponse = await getGroups();
    if (apiResponse.success) {
        dispatch(receiveGroupsData(apiResponse.data));
    } else {
        dispatch(stopRequestGroupsWithError(apiResponse.errors));
    }
}

export const groupActionCreators = {
    startRequestGroups,
    requestGroups
};