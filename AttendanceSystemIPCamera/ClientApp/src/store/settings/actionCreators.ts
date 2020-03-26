import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import Setting from '../../models/Setting';
import * as services from "../../services/settings";

export const ACTIONS = {
    CHECK_FOR_UPDATES: 'CHECK_FOR_UPDATES',
    RECEIVE_CHECK_UPDATES: 'RECEIVE_CHECK_UPDATES',
    UPDATE: 'UPDATE'
};

export interface UpdatableSettings {
    room: Setting;
    unit: Setting;
    model: Setting;
    others: Setting;
};

function receiveCheckUpdates(settings: UpdatableSettings) {
    return {
        type: ACTIONS.RECEIVE_CHECK_UPDATES,
        settings
    };
}

const checkForUpdates = (): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await services.checkForUpdates();
    dispatch(receiveCheckUpdates(apiResponse.data));
};

export const settingActionCreators = {
    checkForUpdates
};