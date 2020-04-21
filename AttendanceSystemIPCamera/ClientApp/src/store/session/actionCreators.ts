import ApiResponse from '../../models/ApiResponse';
import { AppThunkAction } from '..';
import {
	getSession,
	getSessionAttendeeRecordList,
	getActiveSession,
	exportSession,
	getPastSession,
	getSessionUnknownImagesList,
	createScheduledSessions,
	getScheduledSesionByGroupCode,
	deleteScheduledSession,
	updateRoom
} from '../../services/session';
import Session from '../../models/Session';
import Record from '../../models/Record';
import Attendee from '../../models/Attendee';
import AttendeeRecordPair from '../../models/AttendeeRecordPair';
import ExportRequest from '../../models/ExportRequest'
import UpdateRecord from '../../models/UpdateRecord';
import { updateRecord } from '../../services/record';
import ScheduleCreate from '../../models/ScheduleCreate';
import { success, warning, error } from '../../utils';
import SessionUpdateRoom from '../../models/SessionUpdateRoom';

export const ACTIONS = {
    RECEIVE_ACTIVE_SESSION: 'RECEIVE_ACTIVE_SESSION',
	START_REQUEST_SESSION: 'START_REQUEST_SESSION',
	STOP_REQUEST_SESSION_WITH_ERRORS: 'STOP_REQUEST_SESSION_WITH_ERRORS',
	RECEIVE_SESSION_DATA: 'RECEIVE_SESSION_DATA',
	START_REQUEST_ATTENDEE_RECORDS: 'START_REQUEST_ATTENDEE_RECORDS',
	STOP_REQUEST_ATTENDEE_RECORDS_WITH_ERRORS:
		'STOP_REQUEST_ATTENDEE_RECORDS_WITH_ERRORS',
	RECEIVE_ATTENDEE_RECORDS_DATA: 'RECEIVE_ATTENDEE_RECORDS_DATA',
	UPDATE_ATTENDEE_RECORD_SEARCH: 'UPDATE_ATTENDEE_RECORD_SEARCH',
	UPDATE_ATTENDEE_RECORD: 'UPDATE_ATTENDEE_RECORD',
	UPDATE_ATTENDEE_RECORD_REAL_TIME: 'UPDATE_ATTENDEE_RECORD_REAL_TIME',
	START_REAL_TIME_CONNECTION: 'START_REAL_TIME_CONNECTION',
	START_TAKING_ATTENDANCE: 'START_TAKING_ATTENDANCE',
	END_TAKING_ATTENDANCE: 'END_TAKING_ATTENDANCE',
	RECEIVE_UNKNOWN_IMAGES: 'RECEIVE_UNKNOWN_IMAGES',
	UPDATE_UNKNOWN_REAL_TIME: 'UPDATE_UNKNOWN_REAL_TIME',
	REMOVE_UNKNOWN_IMAGE: 'REMOVE_UNKNOWN_IMAGE',
	UPDATE_UNKNOWN_REAL_TIME_BATCH: 'UPDATE_UNKNOWN_REAL_TIME_BATCH',
	UPDATE_ATTENDEE_RECORD_REAL_TIME_BATCH: 'UPDATE_ATTENDEE_RECORD_REAL_TIME_BATCH'
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

function receiveSessionUnknownImages(unknownImages: string[]) {
	return {
		type: ACTIONS.RECEIVE_UNKNOWN_IMAGES,
		unknownImages
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

const requestUnknownImages = (
	sessionId: number
): AppThunkAction => async dispatch => {
	const apiResponse: ApiResponse = await getSessionUnknownImagesList(
		sessionId
	);
	if (apiResponse.success) {
		dispatch(receiveSessionUnknownImages(apiResponse.data));
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
		dispatch(requestUnknownImages(sessionId) as any);
	} else {
		dispatch(stopRequestGroupsWithError(apiResponse.errors));
	}
	};

function updateAttendeeRecord(updateInfo: UpdateRecord, updatedRecord: Record) {
	return {
		type: ACTIONS.UPDATE_ATTENDEE_RECORD,
		updateInfo,
		updatedRecord
	};
}

const createOrUpdateRecord = (
	updateInfo: UpdateRecord,
	assumeSuccess: boolean = true
): AppThunkAction => async dispatch => {
	if (assumeSuccess) {
		const temporaryUpdatedRecord: Record = {
			id: -1,
			attendee: {
				code: updateInfo.attendeeCode,
				name: '',
				image: ''
			},
			present: updateInfo.present
		};
		dispatch(updateAttendeeRecord(updateInfo, temporaryUpdatedRecord));
	}
	const apiResponse: ApiResponse = await updateRecord(updateInfo);

	if (apiResponse.success) {
		dispatch(updateAttendeeRecord(updateInfo, apiResponse.data));
	} else {
		// TODO: show error here
	}
	};

function updateAttendeeRecordRealTime(attendeeCode: string) {
	return {
		type: ACTIONS.UPDATE_ATTENDEE_RECORD_REAL_TIME,
		attendeeCode
	};
}

function receiveActiveSession(activeSession: any) {
    return {
        type: ACTIONS.RECEIVE_ACTIVE_SESSION,
        activeSession
    };
}

export const requestActiveSession = (): AppThunkAction => async (dispatch, getState) => {
	const apiResponse: ApiResponse = await getActiveSession();
	dispatch(receiveActiveSession(apiResponse.data));
};

export const startGenerateExport = (exportRequest: ExportRequest, success: Function, setData: Function): AppThunkAction => async (dispatch, getState) => {
	const apiResponse: ApiResponse = await exportSession(exportRequest);
	if (apiResponse.success) {
		success(exportRequest);
		setData(apiResponse.data, exportRequest);
	} else {
		console.log(apiResponse.errors);
	}
}

export const startGetPastSession = (groupCode: string, loadSession: Function): AppThunkAction => async (dispatch, getState) => {
	const apiResponse: ApiResponse = await getPastSession(groupCode);
	if (apiResponse.success) {
		loadSession(apiResponse.data);
	} else {
		console.log(apiResponse.errors);
	}
}

export const requestCreateScheduledSession = (schedules: ScheduleCreate[], reloadData: Function): AppThunkAction => async (dispatch, getState) => {
	const apiResponse: ApiResponse = await createScheduledSessions(schedules);
	if(apiResponse.success) {
        var countCreatedItem = apiResponse.data.length;
        if(countCreatedItem > 0) {
            success("Create " + countCreatedItem + " schedules successfully!");
        } else {
            warning("Data is not valid. Created failed!");
        }
        reloadData();
    } else {
        error("Oops.. something went wrong!");
        console.log(apiResponse.errors);
    }
}

export const requestGetScheduledSessionByGroupCode = (groupCode: string, loadData: Function): AppThunkAction => async (dispatchEvent, getState) => {
    const apiResponse: ApiResponse = await getScheduledSesionByGroupCode(groupCode);
    if(apiResponse.success) {
        loadData(apiResponse.data);
    } else {
        error("Oops.. something went wrong!");
        console.log(apiResponse.errors);
    }
}

export const requestDeleteScheduledSession = (scheduledId: number, reloadSchedule: Function): AppThunkAction => async (dispatchEvent, getState) => {
	const apiResponse: ApiResponse = await deleteScheduledSession(scheduledId);
	if(apiResponse.success) {
		success("Delete schedule success!");
		reloadSchedule();
	} else {
		error("Delete schedule falied!");
		console.log(apiResponse.errors);
	}
}

export const requestUpdateRoom = (data: SessionUpdateRoom): AppThunkAction => async (dispatchEvent, getState) => {
	const apiResponse: ApiResponse = await updateRoom(data);
	if(apiResponse.success) {
		dispatchEvent(receiveSessionData(apiResponse.data));
		success("Update room successfully!");
	} else {
		error("Update room failed!");
		console.log(apiResponse.errors);
	}
}

function startRealTimeConnection() {
	return {
		type: ACTIONS.START_REAL_TIME_CONNECTION,
	};
}

function endTakingAttendance() {
	return {
		type: ACTIONS.END_TAKING_ATTENDANCE
	};
}

function startTakingAttendance(session: any) {
	return {
		type: ACTIONS.START_TAKING_ATTENDANCE,
		session
	};
}

function updateUnknownRealTime(image: any) {
	return {
		type: ACTIONS.UPDATE_UNKNOWN_REAL_TIME,
		image
	};
}

function removeUnknownImage(image: string) {
	return {
		type: ACTIONS.REMOVE_UNKNOWN_IMAGE,
		image
	};
}

function updateUnknownRealTimeBatch(images: string[]) {
	return {
		type: ACTIONS.UPDATE_UNKNOWN_REAL_TIME_BATCH,
		images
	};
}

function updateAttendeeRecordRealTimeBatch(attendeeCodes: string[]) {
	return {
		type: ACTIONS.UPDATE_ATTENDEE_RECORD_REAL_TIME_BATCH,
		attendeeCodes
	};
}

export const sessionActionCreators = {
	requestSession,
	createOrUpdateRecord,
	updateAttendeeRecordRealTime,
	requestActiveSession,
	startGenerateExport,
	startGetPastSession,
	startRealTimeConnection,
	startTakingAttendance,
	endTakingAttendance,
	updateUnknownRealTime,
	removeUnknownImage,
	updateAttendeeRecordRealTimeBatch,
	updateUnknownRealTimeBatch,
	requestCreateScheduledSession,
	requestGetScheduledSessionByGroupCode,
	requestDeleteScheduledSession,
	requestUpdateRoom
};
