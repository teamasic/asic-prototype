import Group from "../../models/Group";
import ApiResponse from "../../models/ApiResponse";
import { getGroups } from "../../services/group";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import PaginatedList from "../../models/PaginatedList";
import GroupSearch from "../../models/GroupSearch";

export const ACTIONS = {
    START_REQUEST_GROUPS: 'START_REQUEST_GROUPS',
    STOP_REQUEST_GROUPS_WITH_ERRORS: 'STOP_REQUEST_GROUPS_WITH_ERRORS',
    RECEIVE_GROUPS_DATA: 'RECEIVE_GROUPS_DATA'
}

function startRequestGroups(groupSearch: GroupSearch) {
    return {
        type: ACTIONS.START_REQUEST_GROUPS,
        groupSearch
    };
}

function stopRequestGroupsWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_GROUPS_WITH_ERRORS,
        errors
    };
}

function receiveGroupsData(paginatedGroupList: PaginatedList<Group>) {
    console.log(paginatedGroupList);
    return {
        type: ACTIONS.RECEIVE_GROUPS_DATA,
        paginatedGroupList
    };
}

const requestGroups = (groupSearch: GroupSearch): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestGroups(groupSearch));

    const apiResponse: ApiResponse = await getGroups(groupSearch);
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