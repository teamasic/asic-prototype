﻿import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import ChangeRequest, { ChangeRequestStatusFilter } from '../../models/ChangeRequest';
import * as services from "../../services/changeRequest";

export const ACTIONS = {
    RECEIVE_CHANGE_REQUEST_DATA: 'RECEIVE_CHANGE_REQUEST_DATA',
    START_REQUEST_CHANGE_REQUESTS: 'START_REQUEST_CHANGE_REQUESTS',
    STOP_REQUEST_CHANGE_REQUESTS_WITH_ERRORS: 'STOP_REQUEST_CHANGE_REQUESTS_WITH_ERRORS',
    PROCESS_CHANGE_REQUEST: 'PROCESS_CHANGE_REQUEST'
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

function receiveChangeRequests(changeRequests: ChangeRequest[]) {
    return {
        type: ACTIONS.RECEIVE_CHANGE_REQUEST_DATA,
        changeRequests
    };
}

export const requestChangeRequests = (status: ChangeRequestStatusFilter): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestChangeRequests());

    const apiResponse: ApiResponse = await services.getChangeRequestList(status);
    if (apiResponse.success) {
        dispatch(receiveChangeRequests(apiResponse.data));
    } else {
        dispatch(stopRequestChangeRequestsWithError(apiResponse.errors));
    }
};

function receiveProcessChangeRequest(id: number, approved: boolean) {
    return {
        type: ACTIONS.PROCESS_CHANGE_REQUEST,
        id,
        approved
    };
}

export const processChangeRequest = (id: number, approved: boolean): AppThunkAction => async (dispatch, getState) => {
    dispatch(receiveProcessChangeRequest(id, approved));
    const apiResponse: ApiResponse = await services.processChangeRequest(id, approved);
    if (apiResponse.success) {
        dispatch(receiveProcessChangeRequest(id, approved));
    }
};

export const changeRequestActionCreators = {
    requestChangeRequests,
    processChangeRequest
};