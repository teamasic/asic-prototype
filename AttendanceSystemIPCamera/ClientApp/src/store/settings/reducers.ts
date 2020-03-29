import { Reducer, Action, AnyAction } from "redux";
import { SettingState } from "./state";
import { ACTIONS } from "./actionCreators";
import Setting from "../../models/Setting";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedSetting: Setting = {
    needsUpdate: false,
    lastUpdated: new Date(),
    newestServerUpdate: new Date(),
    loading: false
};
const unloadedState: SettingState = {
    model: {
        ...unloadedSetting
    },
    room: {
        ...unloadedSetting
    },
    unit: {
        ...unloadedSetting
    },
    others: {
        ...unloadedSetting
    },
};

const reducers: Reducer<SettingState> = (state: SettingState | undefined, incomingAction: AnyAction): SettingState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.RECEIVE_CHECK_UPDATES:
            return {
                ... state,
                ... action.settings
            };
        case ACTIONS.RECEIVE_UPDATE:
            return {
                ...state,
                [action.key]: {
                    ...action.setting,
                    loading: false
                }
            };
        case ACTIONS.START_LOADING_UPDATE:
            return {
                ...state,
                [action.key]: {
                    ...(state as any)[action.key],
                    loading: true
                }
            }
        case ACTIONS.STOP_LOADING_UPDATE:
            return {
                ...state,
                [action.key]: {
                    ...(state as any)[action.key],
                    loading: false
                }
            }
    }

    return state;
};

export default reducers;