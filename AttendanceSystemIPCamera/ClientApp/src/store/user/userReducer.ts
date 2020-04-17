import { Reducer, Action, AnyAction } from "redux";
import { UserState } from "./userState";
import { ACTIONS } from "./userActionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: UserState = {
    isLoading: false,
    successfullyLoaded: false,
    currentUser: {
        code: "",
        name: "",
        email: "",
        image: ""
    },
    isLogin: false,
    errors:[]
};

const reducers: Reducer<UserState> = (state: UserState | undefined, incomingAction: AnyAction): UserState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.START_REQUEST_LOGIN:
            return {
                ...state,
                isLoading: true,
                successfullyLoaded: false,
            };
        case ACTIONS.STOP_REQUEST_LOGIN_WITH_ERRORS:
            return {
                ...state,
                isLoading: false,
                successfullyLoaded: true,
                errors: action.errors
            };
        case ACTIONS.RECEIVE_SUCCESS_LOGIN:
            return {
                ...state,
                currentUser: action.user,
                // roles: action.roles,
                // accessToken: action.accessToken,
                isLoading: false,
                successfullyLoaded: true,
                isLogin: true,
                errors: []
            };
        case ACTIONS.USER_INFO_NOT_IN_LOCAL:
            return {
                ...state,
                isLogin: false,
                isLoading: false,
                successfullyLoaded: true
            }
        case ACTIONS.LOG_OUT:
            return {
                ...state,
                ...unloadedState,
            }
    }

    return state;
};

export default reducers;