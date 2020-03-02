import { Reducer, Action, AnyAction } from "redux";
import { ChangeRequestState } from "./state";
import { ACTIONS } from "./actionCreators";
import { ChangeRequestStatus } from "../../models/ChangeRequest";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: ChangeRequestState = {
    changeRequests: [],
    isLoading: false,
    successfullyLoaded: false
};

const reducers: Reducer<ChangeRequestState> = (
    state: ChangeRequestState | undefined, incomingAction: AnyAction): ChangeRequestState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.START_REQUEST_CHANGE_REQUESTS:
            return {
                ...state,
                isLoading: true
            };
        case ACTIONS.STOP_REQUEST_CHANGE_REQUESTS_WITH_ERRORS:
            return {
                ...state,
                isLoading: false,
                successfullyLoaded: false,
                changeRequests: []
            }
        case ACTIONS.RECEIVE_CHANGE_REQUEST_DATA:
            return {
                ...state,
                isLoading: false,
                successfullyLoaded: true,
                changeRequests: action.changeRequests
            };
        case ACTIONS.PROCESS_CHANGE_REQUEST:
            return {
                ...state,
                changeRequests: state.changeRequests.map(cr => cr.id === action.id ? {
                    ...cr,
                    status: action.approved ? ChangeRequestStatus.APPROVED : ChangeRequestStatus.REJECTED
                } : cr)
            }
    }

    return state;
};

export default reducers;