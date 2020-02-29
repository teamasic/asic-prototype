import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import ChangeRequest, { ChangeRequestStatusFilter } from '../../models/ChangeRequest';
import * as services from "../../services/changeRequest";

export const ACTIONS = {
    RECEIVE_CHANGE_REQUEST_DATA: 'RECEIVE_CHANGE_REQUEST_DATA'
}
function receiveChangeRequests(changeRequests: ChangeRequest[]) {
    return {
        type: ACTIONS.RECEIVE_CHANGE_REQUEST_DATA,
        changeRequests
    };
}

export const requestChangeRequests = (status: ChangeRequestStatusFilter): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await services.getChangeRequestList(status);
    dispatch(receiveChangeRequests(apiResponse.data));
}

export const unitActionCreators = {
    requestChangeRequests
};