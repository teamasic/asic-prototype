import { Reducer, Action, AnyAction } from "redux";
import { SessionsState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SessionsState = {
    activeSession: {},
};

const reducers: Reducer<SessionsState> = (state: SessionsState | undefined, incomingAction: AnyAction): SessionsState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.RECEIVE_ACTIVE_SESSION:
            return {
                ... state,
                activeSession: action.activeSession
            };
    }

    return state;
};

export default reducers;