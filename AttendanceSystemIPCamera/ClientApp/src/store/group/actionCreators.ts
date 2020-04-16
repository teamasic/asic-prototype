import Group from "../../models/Group";
import ApiResponse from "../../models/ApiResponse";
import {
    getGroups, createGroup, getGroupDetail,
    deactiveGroup, updateGroup, deleteAttendeeGroup,
    createAttendeeInGroup
} from "../../services/group";
import { getAttendeeByCode } from "../../services/attendee"
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import PaginatedList from "../../models/PaginatedList";
import GroupSearch from "../../models/GroupSearch";
import Attendee from "../../models/Attendee";
import { STATUS_CODES } from "http";
import HttpStatusCode from "../../models/HttpStatusCode";
import { success, error } from "../../utils";

export const ACTIONS = {
    START_REQUEST_GROUPS: 'START_REQUEST_GROUPS',
    STOP_REQUEST_GROUPS_WITH_ERRORS: 'STOP_REQUEST_GROUPS_WITH_ERRORS',
    RECEIVE_GROUPS_DATA: 'RECEIVE_GROUPS_DATA',
    CREATE_NEW_GROUP: 'CREATE_NEW_GROUP',
    CREATE_NEW_GROUP_SUCCESS: 'CREATE_NEW_GROUP_SUCCESS',
    RECEIVE_GROUP_DETAIL: 'RECEIVE_GROUP_DETAIL',
    UPDATE_GROUP_NAME_SUCCESS: 'UPDATE_GROUP_NAME_SUCCESS',
    DELETE_ATTENDEE_GROUP_SUCCESS: 'DELETE_ATTENDEE_GROUP_SUCCESS',
    CREATE_ATTENDEE_IN_GROUP_SUCCESS: 'CREATE_ATTENDEE_IN_GROUP_SUCCESS'
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

function updateGroupNameSuccess(updatedGroup: Group) {
    return {
        type: ACTIONS.UPDATE_GROUP_NAME_SUCCESS,
        updatedGroup: updatedGroup
    }
}

function refreshListAttendeeAfterDeleteSuccess(deletedAttendeeCode: string) {
    return {
        type: ACTIONS.DELETE_ATTENDEE_GROUP_SUCCESS,
        attendeeCode: deletedAttendeeCode
    }
}

function createAttendeeInGroupSuccess(newAttendee: Attendee) {
    return {
        type: ACTIONS.CREATE_ATTENDEE_IN_GROUP_SUCCESS,
        newAttendee: newAttendee
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

const requestGroupDetail = (code: string, stopLoadingTable: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getGroupDetail(code);
    if (apiResponse.success) {
        dispatch(receiveGroupDetail(apiResponse.data));
        stopLoadingTable();
    } else {
        console.log("Get group detail error: " + apiResponse.errors);
    }
}

const postGroup = (newGroup: Group, reloadGroups: Function, resetModal: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startCreateNewGroup(newGroup));
    const apiResponse: ApiResponse = await createGroup(newGroup);
    if (apiResponse.success) {
        dispatch(createGroupSuccess(apiResponse.data));
        success("Create group success!");
        reloadGroups();
    } else {
        switch (apiResponse.statusCode) {
            case HttpStatusCode.BAD_REQUEST:
                error("Group information is invalid!");
                break;
            case HttpStatusCode.EXISTED:
                error("Group with code " + newGroup.code + " is already existed!");
                break;
            default:
                error("Oops.. Something went wrong!");
        }
    }
    resetModal();
}

const startDeactiveGroup = (code: string, groupSearch: GroupSearch, success: Function, error: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await deactiveGroup(code);
    if (apiResponse.success) {
        success();
        const groupResponse: ApiResponse = await getGroups(groupSearch);
        if (groupResponse.success) {
            dispatch(receiveGroupsData(groupResponse.data));
        } else {
            dispatch(stopRequestGroupsWithError(groupResponse.errors));
        }
    } else {
        error("Oops..Something went wrong!");
        console.log(apiResponse.errors);
    }
}

const startUpdateGroup = (group: Group, success: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await updateGroup(group.code, group);
    if (apiResponse.success) {
        dispatch(updateGroupNameSuccess(apiResponse.data));
        success();
    } else {
        console.log(apiResponse.errors.toString());
    }
}

const startDeleteAttendeeGroup = (attendeeCode: string, groupCode: string, success: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await deleteAttendeeGroup(attendeeCode, groupCode);
    if (apiResponse.success) {
        console.log(apiResponse.data.attendeeCode);
        dispatch(refreshListAttendeeAfterDeleteSuccess(apiResponse.data.attendeeCode));
        success();
    } else {
        console.log(apiResponse.errors.toString());
    }
}

const startGetAttendeeByCode = (code: string, loadName: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getAttendeeByCode(code);
    if (apiResponse.success) {
        if (apiResponse.data != null) {
            loadName(apiResponse.data.name);
        } else {
            loadName("");
        }
    } else {
        console.log("Get attendee by code error: " + apiResponse.errors.toString());
    }
}

const startCreateAttendeeInGroup = (groupCode: string, newAttendee: Attendee, success: Function, duplicateAttendee: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await createAttendeeInGroup(groupCode, newAttendee);
    if (apiResponse.success) {
        dispatch(createAttendeeInGroupSuccess(apiResponse.data));
        success();
    } else {
        duplicateAttendee();
        console.log(apiResponse.errors);
    }
}

export const groupActionCreators = {
    postGroup,
    requestGroups,
    requestGroupDetail,
    startRequestGroups,
    startDeactiveGroup,
    startUpdateGroup,
    startDeleteAttendeeGroup,
    startGetAttendeeByCode,
    startCreateAttendeeInGroup
};