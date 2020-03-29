import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import Setting from '../../models/Setting';
import * as services from "../../services/settings";

export const ACTIONS = {
    CHECK_FOR_UPDATES: 'CHECK_FOR_UPDATES',
    RECEIVE_CHECK_UPDATES: 'RECEIVE_CHECK_UPDATES',
    UPDATE: 'UPDATE',
    START_LOADING_UPDATE: 'START_LOADING_UPDATE',
    STOP_LOADING_UPDATE: 'STOP_LOADING_UPDATE',
    RECEIVE_UPDATE: 'RECEIVE_UPDATE'
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

function receiveUpdate(key: string, setting: Setting) {
    return {
        type: ACTIONS.RECEIVE_UPDATE,
        key,
        setting
    };
}

function startLoadingUpdate(key: string) {
    return {
        type: ACTIONS.START_LOADING_UPDATE,
        key
    };
}

function stopLoadingUpdate(key: string) {
    return {
        type: ACTIONS.STOP_LOADING_UPDATE,
        key
    };
}

const checkForUpdates = (error: Function): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await services.checkForUpdates();
    if (apiResponse.success) {
        dispatch(receiveCheckUpdates(apiResponse.data));
    } else {
        error();
    }
};

const update = (key: string, success: Function, error: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startLoadingUpdate(key));
    const apiResponse: ApiResponse = await services.update(key);
    if (apiResponse.success) {
        dispatch(receiveUpdate(key, apiResponse.data));
        success();
    } else {
        dispatch(stopLoadingUpdate(key));
        error();
    }
};

export const settingActionCreators = {
    checkForUpdates,
    update
};