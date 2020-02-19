import { Reducer, Action, AnyAction } from "redux";
import { GroupsState } from "./state";
import { ACTIONS } from "./actionCreators";
import { Empty } from "antd";

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
    activeSession: { // temporary, might be restructured
        id: 1,
        startTime: new Date(),
        attendees: [{
            id: 1,
            code: 'SE63147',
            name: 'Strawberry'
        }, {
            id: 2,
            code: 'SE63147',
            name: 'Blackberry'
        }]
    },
    selectedGroup: { // temporary, might be restructured
        id: 1,
        code: 'None',
        name: 'None',
        attendees: [],
        lastSessionTime: undefined,
        sessions: [{
            id: 1,
            startTime: new Date(),
            attendees: [{
                id: 1,
                code: 'SE63147',
                name: 'Strawberry'
            }, {
                id: 2,
                code: 'SE63147',
                name: 'Blackberry'
            }]
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
        case ACTIONS.CREATE_NEW_GROUP:
            return {
                ...state
            }
        case ACTIONS.RECEIVE_GROUP_DETAIL:
            return {
                ...state,
                selectedGroup: action.groupDetail
            }
    }

    return state;
};

export default reducers;