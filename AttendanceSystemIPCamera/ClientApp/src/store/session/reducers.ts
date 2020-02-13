import { Reducer, Action, AnyAction } from 'redux';
import { SessionState } from './state';
import { ACTIONS } from './actionCreators';

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SessionState = {
	isLoadingSession: false,
	successfullyLoadedSession: false,
	activeSession: undefined,
	isLoadingAttendeeRecords: false,
	successfullyLoadedAttendeeRecords: false,
	attendeeRecords: []
};

const reducers: Reducer<SessionState> = (
	state: SessionState | undefined,
	incomingAction: AnyAction
): SessionState => {
	if (state === undefined) {
		return unloadedState;
	}

	const action = incomingAction;
	console.log(action);
	switch (action.type) {
		case ACTIONS.START_REQUEST_SESSION:
			return {
				...state,
				isLoadingSession: true,
				successfullyLoadedSession: false,
				activeSession: undefined,
				isLoadingAttendeeRecords: false,
				successfullyLoadedAttendeeRecords: false,
				attendeeRecords: []
			};
		case ACTIONS.STOP_REQUEST_SESSION_WITH_ERRORS:
			return {
				...state,
				isLoadingSession: false,
				successfullyLoadedSession: false
			};
		case ACTIONS.RECEIVE_SESSION_DATA:
			return {
				...state,
				isLoadingSession: false,
				successfullyLoadedSession: true,
				activeSession: action.session
			};
		case ACTIONS.START_REQUEST_ATTENDEE_RECORDS:
			return {
				...state,
				isLoadingAttendeeRecords: true,
				successfullyLoadedAttendeeRecords: false,
				attendeeRecords: []
			};
		case ACTIONS.STOP_REQUEST_ATTENDEE_RECORDS_WITH_ERRORS:
			return {
				...state,
				isLoadingAttendeeRecords: false,
				successfullyLoadedAttendeeRecords: false,
				attendeeRecords: []
			};
		case ACTIONS.RECEIVE_ATTENDEE_RECORDS_DATA:
			return {
				...state,
				isLoadingAttendeeRecords: false,
				successfullyLoadedAttendeeRecords: true,
				attendeeRecords: action.attendeeRecords
			};
	}

	return state;
};

export default reducers;
