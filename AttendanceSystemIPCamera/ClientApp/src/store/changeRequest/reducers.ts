import { Reducer, Action, AnyAction } from "redux";
import { ChangeRequestState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: ChangeRequestState = {
    changeRequests: [],
};

const reducers: Reducer<ChangeRequestState> = (
    state: ChangeRequestState | undefined, incomingAction: AnyAction): ChangeRequestState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.RECEIVE_CHANGE_REQUEST_DATA:
            return {
                ...state,
                changeRequests: action.changeRequests
            };
    }

    return state;
};

export default reducers;