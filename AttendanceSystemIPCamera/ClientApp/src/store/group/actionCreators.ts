﻿import Group from "../../models/Group";
import ApiResponse from "../../models/ApiResponse";
import { getGroups, createGroup, getGroupDetail } from "../../services/group";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import PaginatedList from "../../models/PaginatedList";
import GroupSearch from "../../models/GroupSearch";

export const ACTIONS = {
    START_REQUEST_GROUPS: 'START_REQUEST_GROUPS',
    STOP_REQUEST_GROUPS_WITH_ERRORS: 'STOP_REQUEST_GROUPS_WITH_ERRORS',
    RECEIVE_GROUPS_DATA: 'RECEIVE_GROUPS_DATA',
    CREATE_NEW_GROUP: 'CREATE_NEW_GROUP',
    CREATE_NEW_GROUP_SUCCESS: 'CREATE_NEW_GROUP_SUCCESS',
    RECEIVE_GROUP_DETAIL: 'RECEIVE_GROUP_DETAIL'
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

// normal action creator // can't use async, call API
function receiveGroupsData(paginatedGroupList: PaginatedList<Group>) {
    return {
        type: ACTIONS.RECEIVE_GROUPS_DATA,
        paginatedGroupList
    };
}

function startCreateNewGroup(newGroup: Group) {
    return {
        type: ACTIONS.CREATE_NEW_GROUP
    }
}

function receiveGroupDetail(groupDetail: Group) {
    return {
        type: ACTIONS.RECEIVE_GROUP_DETAIL,
        groupDetail: groupDetail
    }
}

function createGroupSuccess(newGroup: Group) {
    return {
        type: ACTIONS.CREATE_NEW_GROUP_SUCCESS,
        newGroup: newGroup
    }
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

const requestGroupDetail = (id: number, renderDetailPage: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getGroupDetail(id);
    if (apiResponse.success) {
        dispatch(receiveGroupDetail(apiResponse.data));
        renderDetailPage();
    } else {
        console.log("Get group detail error: " + apiResponse.errors);
    }
}

const postGroup = (newGroup: Group, renderDetailPage: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startCreateNewGroup(newGroup));
    const apiResponse: ApiResponse = await createGroup(newGroup);
    if (apiResponse.success) {
        dispatch(createGroupSuccess(apiResponse.data));
        renderDetailPage();
    } else {
        console.log("Create group error: " + apiResponse.errors);
    }
}

export const groupActionCreators = {
    postGroup,
    requestGroups,
    requestGroupDetail,
    startRequestGroups
};