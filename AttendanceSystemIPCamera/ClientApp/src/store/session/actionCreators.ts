import ApiResponse from '../../models/ApiResponse';
import { AppThunkAction } from '..';
import {
	getSession,
	getSessionAttendeeRecordList
} from '../../services/session';
import Session from '../../models/Session';
import AttendeeRecordPair from '../../models/AttendeeRecordPair';

export const ACTIONS = {
	START_REQUEST_SESSION: 'START_REQUEST_SESSION',
	STOP_REQUEST_SESSION_WITH_ERRORS: 'STOP_REQUEST_SESSION_WITH_ERRORS',
	RECEIVE_SESSION_DATA: 'RECEIVE_SESSION_DATA',
	START_REQUEST_ATTENDEE_RECORDS: 'START_REQUEST_ATTENDEE_RECORDS',
	STOP_REQUEST_ATTENDEE_RECORDS_WITH_ERRORS:
		'STOP_REQUEST_ATTENDEE_RECORDS_WITH_ERRORS',
	RECEIVE_ATTENDEE_RECORDS_DATA: 'RECEIVE_ATTENDEE_RECORDS_DATA'
};

function startRequestSession(sessionId: number) {
	return {
		type: ACTIONS.START_REQUEST_SESSION,
		sessionId
	};
}

function stopRequestGroupsWithError(errors: any[]) {
	return {
		type: ACTIONS.STOP_REQUEST_SESSION_WITH_ERRORS,
		errors
	};
}

function receiveSessionData(session: Session) {
	return {
		type: ACTIONS.RECEIVE_SESSION_DATA,
		session
	};
}

function startRequestAttendeeRecords(sessionId: number) {
	return {
		type: ACTIONS.START_REQUEST_ATTENDEE_RECORDS,
		sessionId
	};
}

function stopRequestAttendeeRecordsWithError(errors: any[]) {
	return {
		type: ACTIONS.STOP_REQUEST_ATTENDEE_RECORDS_WITH_ERRORS,
		errors
	};
}

function receiveSessionAttendeeRecords(attendeeRecords: AttendeeRecordPair[]) {
	return {
		type: ACTIONS.RECEIVE_ATTENDEE_RECORDS_DATA,
		attendeeRecords
	};
}

const requestAttendeeRecords = (
	sessionId: number
): AppThunkAction => async dispatch => {
	dispatch(startRequestAttendeeRecords(sessionId));

	const apiResponse: ApiResponse = await getSessionAttendeeRecordList(
		sessionId
	);
	if (apiResponse.success) {
		dispatch(receiveSessionAttendeeRecords(apiResponse.data));
	} else {
		dispatch(stopRequestAttendeeRecordsWithError(apiResponse.errors));
	}
};

const requestSession = (
	sessionId: number
): AppThunkAction => async dispatch => {
	dispatch(startRequestSession(sessionId));

	const apiResponse: ApiResponse = await getSession(sessionId);

	if (apiResponse.success) {
		dispatch(requestAttendeeRecords(sessionId) as any);
		dispatch(receiveSessionData(apiResponse.data));
	} else {
		dispatch(stopRequestGroupsWithError(apiResponse.errors));
	}
};

export const sessionActionCreators = {
	requestSession
};
