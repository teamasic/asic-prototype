import { Reducer, Action, AnyAction } from "redux";
import { NotificationState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: NotificationState = {
    notifications: []
};

const reducers: Reducer<NotificationState> = (state: NotificationState | undefined,
    incomingAction: AnyAction): NotificationState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.RECEIVE_NOTIFICATION:
            return {
                ...state,
                notifications: [...state.notifications, action.notification]
            };
    }

    return state;
};

export default reducers;