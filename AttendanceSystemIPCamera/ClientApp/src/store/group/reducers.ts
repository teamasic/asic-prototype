import { Reducer, Action, AnyAction } from "redux";
import { GroupsState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: GroupsState = {
    groups: [],
    isLoading: false,
    successfullyLoaded: false
};

const reducers: Reducer<GroupsState> = (state: GroupsState | undefined, incomingAction: AnyAction): GroupsState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.START_REQUEST_GROUPS:
            return {
                ... state,
                isLoading: true,
                successfullyLoaded: false
            };
        case ACTIONS.STOP_REQUEST_GROUPS_WITH_ERRORS:
            return {
                groups: [],
                isLoading: false,
                successfullyLoaded: false
            };
        case ACTIONS.RECEIVE_GROUPS_DATA:
            return {
                groups: action.groups,
                isLoading: false,
                successfullyLoaded: true
            };
    }

    return state;
};

export default reducers;