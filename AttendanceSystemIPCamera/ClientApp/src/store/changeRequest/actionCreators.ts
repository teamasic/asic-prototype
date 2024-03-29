﻿import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import ChangeRequest, { ChangeRequestStatusFilter } from '../../models/ChangeRequest';
import * as services from "../../services/changeRequest";

export const ACTIONS = {
    RECEIVE_CHANGE_REQUEST_DATA: 'RECEIVE_CHANGE_REQUEST_DATA',
    START_REQUEST_CHANGE_REQUESTS: 'START_REQUEST_CHANGE_REQUESTS',
    STOP_REQUEST_CHANGE_REQUESTS_WITH_ERRORS: 'STOP_REQUEST_CHANGE_REQUESTS_WITH_ERRORS',
    PROCESS_CHANGE_REQUEST: 'PROCESS_CHANGE_REQUEST',
    INCREMENT_UNRESOLVED_COUNT: 'INCREMENT_UNRESOLVED_COUNT'
}

function startRequestChangeRequests() {
    return {
        type: ACTIONS.START_REQUEST_CHANGE_REQUESTS
    };
}

function stopRequestChangeRequestsWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_CHANGE_REQUESTS_WITH_ERRORS,
        errors
    };
}

function receiveChangeRequests(changeRequests: ChangeRequest[], filterStatus: ChangeRequestStatusFilter) {
    return {
        type: ACTIONS.RECEIVE_CHANGE_REQUEST_DATA,
        changeRequests,
        filterStatus
    };
}

export const requestChangeRequests = (status: ChangeRequestStatusFilter): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestChangeRequests());

    const apiResponse: ApiResponse = await services.getChangeRequestList(status);
    if (apiResponse.success) {
        dispatch(receiveChangeRequests(apiResponse.data, status));
    } else {
        dispatch(stopRequestChangeRequestsWithError(apiResponse.errors));
    }
};

function receiveProcessChangeRequest(recordId: number, approved: boolean) {
    return {
        type: ACTIONS.PROCESS_CHANGE_REQUEST,
        recordId,
        approved
    };
}

const processChangeRequest = (recordId: number, approved: boolean,
    successCallback?: () => void,
    errorCallback?: () => void): AppThunkAction => async (dispatch, getState) => {
        dispatch(receiveProcessChangeRequest(recordId, approved));
        const apiResponse: ApiResponse = await services.processChangeRequest(recordId, approved);
        if (apiResponse.success) {
            dispatch(receiveProcessChangeRequest(recordId, approved));
            if (successCallback) {
                successCallback();
            }
        } else {
            if (errorCallback) {
                errorCallback();
            }
        }
};


function incrementUnresolvedCount() {
    return {
        type: ACTIONS.INCREMENT_UNRESOLVED_COUNT
    };
}

export const changeRequestActionCreators = {
    requestChangeRequests,
    processChangeRequest,
    incrementUnresolvedCount
};