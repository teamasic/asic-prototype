import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import Room from '../../models/Room';
import { getRooms } from "../../services/room";

export const ACTIONS = {
    RECEIVE_CLASSROOMS_DATA: 'RECEIVE_CLASSROOMS_DATA'
}
function receiveRoomsData(roomList: Room[]) {
    debugger;
    return {
        type: ACTIONS.RECEIVE_CLASSROOMS_DATA,
        roomList
    };
}

export const requestRooms = (): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getRooms();
    dispatch(receiveRoomsData(apiResponse.data));
}

export const roomActionCreators = {
    requestRooms
};