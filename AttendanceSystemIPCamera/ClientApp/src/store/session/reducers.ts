﻿import { Reducer, Action, AnyAction } from 'redux';
import { SessionState } from './state';
import { ACTIONS } from './actionCreators';
import Record from '../../models/Record';

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
		case ACTIONS.UPDATE_ATTENDEE_RECORD:
			return {
				...state,
				attendeeRecords: state.attendeeRecords.map(ar =>
					ar.attendee.id === action.updateInfo.attendeeId ? ({
						attendee: ar.attendee,
						record: action.updatedRecord
					}) : ar)
			};
		case ACTIONS.UPDATE_ATTENDEE_RECORD_REAL_TIME:
			let updatedRecord: Record | undefined;
			const updatedAttendeeRecord = state.attendeeRecords.find(ar => ar.attendee.id === action.attendeeId);
			if (updatedAttendeeRecord) {
				if (updatedAttendeeRecord.record != null) {
					updatedRecord = {
						...updatedAttendeeRecord.record,
						present: true
					};
				} else {
					updatedRecord = {
						id: -1,
						attendee: updatedAttendeeRecord.attendee,
						present: true
					};
				}
				return {
					...state,
					attendeeRecords: state.attendeeRecords.map(ar =>
						ar.attendee.id === action.attendeeId ? ({
							attendee: ar.attendee,
							record: updatedRecord
						}) : ar)
				};
			}
	}
	return state;
};

export default reducers;
