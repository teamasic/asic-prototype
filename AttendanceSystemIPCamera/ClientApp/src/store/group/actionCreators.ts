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

function refreshListAttendeeAfterDeleteSuccess(deletedAttendeeId: number) {
    return {
        type: ACTIONS.DELETE_ATTENDEE_GROUP_SUCCESS,
        attendeeId: deletedAttendeeId
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

const startDeactiveGroup = (id: number, groupSearch: GroupSearch, success: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await deactiveGroup(id);
    if (apiResponse.success) {
        success();
        const groupResponse: ApiResponse = await getGroups(groupSearch);
        if (groupResponse.success) {
            dispatch(receiveGroupsData(groupResponse.data));
        } else {
            dispatch(stopRequestGroupsWithError(groupResponse.errors));
        }
    } else {
        console.log("Delete group error: " + apiResponse.errors.toString());
    }
}

const startUpdateGroup = (group: Group, success: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await updateGroup(group.id, group.name);
    if (apiResponse.success) {
        dispatch(updateGroupNameSuccess(apiResponse.data));
    } else {
        console.log("Update group error: " + apiResponse.errors.toString());
    }
}

const startDeleteAttendeeGroup = (attendeeId: number, groupId: number, success: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await deleteAttendeeGroup(attendeeId, groupId);
    if (apiResponse.success) {
        console.log(apiResponse.data.attendeeId);
        dispatch(refreshListAttendeeAfterDeleteSuccess(apiResponse.data.attendeeId));
        success();
    } else {
        console.log("Delete attendee error: " + apiResponse.errors.toString());
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

const startCreateAttendeeInGroup = (groupId: number, newAttendee: Attendee, success: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await createAttendeeInGroup(groupId, newAttendee);
    if (apiResponse.success) {
        if (apiResponse.data != null) {
            dispatch(createAttendeeInGroupSuccess(apiResponse.data));
            success();
        } else {
            console.log("Attendee is already in group");
        }
    } else {
        console.log("Create attendee in group errors: " + apiResponse.errors.toString());
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