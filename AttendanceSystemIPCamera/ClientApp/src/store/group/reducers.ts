﻿import { Reducer, Action, AnyAction } from "redux";
import { GroupsState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: GroupsState = {
    isLoading: false,
    successfullyLoaded: false,
    groupSearch: {
        nameContains: '',
        orderBy: 'Name',
        page: 1,
        pageSize: 15
    },
    activeSession: {
        id: 1,
        startTime: new Date(),
        attendees: [{
            id: 1,
            name: 'Strawberry'
        }, {
            id: 2,
            name: 'Blackberry'
        }, {
            id: 3,
            name: 'Winterberry'
        }, {
            id: 4,
            name: 'Bitterberry'
        }, {
            id: 5,
            name: 'Blueberry'
        }, {
            id: 6,
            name: 'Mulberry'
        }]
    }
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
                successfullyLoaded: false,
                groupSearch: action.groupSearch
            };
        case ACTIONS.STOP_REQUEST_GROUPS_WITH_ERRORS:
            return {
                ...state,
                isLoading: false,
                successfullyLoaded: false
            };
        case ACTIONS.RECEIVE_GROUPS_DATA:
            return {
                ...state,
                paginatedGroupList: action.paginatedGroupList,
                isLoading: false,
                successfullyLoaded: true
            };
    }

    return state;
};

export default reducers;