import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import Classroom from "../../models/classroom";
import { getClassrooms } from "../../services/classroom";

export const ACTIONS = {
    RECEIVE_CLASSROOMS_DATA: 'RECEIVE_CLASSROOMS_DATA'
}
function receiveClassroomsData(classroomList: Classroom[]) {
    debugger;
    return {
        type: ACTIONS.RECEIVE_CLASSROOMS_DATA,
        classroomList
    };
}

export const requestClassrooms = (): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getClassrooms();
    dispatch(receiveClassroomsData(apiResponse.data));
}

export const classroomActionCreators = {
    requestClassrooms
};