import { Reducer, Action, AnyAction } from 'redux';
import { GroupsState } from './state';
import { ACTIONS } from './actionCreators';

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: GroupsState = {
    isLoading: false,
    successfullyLoaded: false,
    groupSearch: {
        nameContains: '',
        orderBy: 'DateCreated',
        page: 1,
        pageSize: 15
    },
    selectedGroup: { // temporary, might be restructured
        code: 'Loading',
        name: 'Loading',
        totalSession: 0,
        attendees: [],
        sessions: [{
            id: 0,
            active: false,
            name: 'Slot 1',
            startTime: new Date(),
            endTime: new Date(),
            attendees: [],
            groupCode: '',
            room: {
                id: 0,
                name: 'None',
                cameraConnectionString: ''
            },
            status: ""
        }]
    }
};

const reducers: Reducer<GroupsState> = (
	state: GroupsState | undefined,
	incomingAction: AnyAction
): GroupsState => {
	if (state === undefined) {
		return unloadedState;
	}

	const action = incomingAction;
	switch (action.type) {
		case ACTIONS.START_REQUEST_GROUPS:
			return {
				...state,
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
        case ACTIONS.CREATE_NEW_GROUP_SUCCESS:
            return {
                ...state,
                selectedGroup: action.newGroup
            }
        case ACTIONS.RECEIVE_GROUP_DETAIL:
            return {
                ...state,
                selectedGroup: action.groupDetail
            };
        case ACTIONS.UPDATE_GROUP_NAME_SUCCESS:
            return {
                ...state,
                selectedGroup: action.updatedGroup
            }
        case ACTIONS.DELETE_ATTENDEE_GROUP_SUCCESS:
            var selectedGroup = {
                ...state.selectedGroup,
                attendees: state.selectedGroup.attendees.filter(a => a.code !== action.attendeeCode)
            };;
            return {
                ...state,
                selectedGroup: selectedGroup
            }
        case ACTIONS.CREATE_ATTENDEE_IN_GROUP_SUCCESS:
            var selectedGroup = {
                ...state.selectedGroup,
                attendees: state.selectedGroup.attendees.concat(action.newAttendee)
            };;
            return {
                ...state,
                selectedGroup: selectedGroup
            }
	}

	return state;
};

export default reducers;
