import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import Classroom from "../../models/classroom";
import { getClassrooms } from "../../services/classroom";

export const ACTIONS = {
    START_REQUEST_CLASSROOMS: 'START_REQUEST_CLASSROOMS',
    STOP_REQUEST_CLASSROOMS_WITH_ERRORS: 'STOP_REQUEST_CLASSROOMS_WITH_ERRORS',
    RECEIVE_CLASSROOMS_DATA: 'RECEIVE_CLASSROOMS_DATA'
}

function startRequestClassrooms() {
    return {
        type: ACTIONS.START_REQUEST_CLASSROOMS,
    };
}

function stopRequestClassroomsWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_CLASSROOMS_WITH_ERRORS,
        errors
    };
}

function receiveClassroomsData(classroomList: Classroom[]) {
    return {
        type: ACTIONS.RECEIVE_CLASSROOMS_DATA,
        classroomList
    };
}

export const requestClassrooms = (): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestClassrooms());

    const apiResponse: ApiResponse = await getClassrooms();
    if (apiResponse.success) {
        dispatch(receiveClassroomsData(apiResponse.data));
    } else {
        dispatch(stopRequestClassroomsWithError(apiResponse.errors));
    }
}

export const classroomActionCreators = {
    startRequestClassrooms,
    requestClassrooms
};