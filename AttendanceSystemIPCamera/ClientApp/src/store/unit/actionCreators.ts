import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import Unit from '../../models/Unit';
import { getUnits } from "../../services/unit";

export const ACTIONS = {
    RECEIVE_UNITS_DATA: 'RECEIVE_UNITS_DATA'
}
function receiveUnitsData(units: Unit[]) {
    return {
        type: ACTIONS.RECEIVE_UNITS_DATA,
        units
    };
}

export const requestUnits = (): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await getUnits();
    dispatch(receiveUnitsData(apiResponse.data));
}

export const unitActionCreators = {
    requestUnits
};