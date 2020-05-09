import { Reducer, Action, AnyAction } from 'redux';
import { SessionState } from './state';
import { ACTIONS } from './actionCreators';
import Record from '../../models/Record';
import AttendeeRecordPair from '../../models/AttendeeRecordPair';
import UpdateRecord from '../../models/UpdateRecord';

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SessionState = {
	isLoadingSession: false,
	successfullyLoadedSession: false,
	activeSession: undefined,
	currentlyOngoingSession: undefined,
	isLoadingAttendeeRecords: false,
	successfullyLoadedAttendeeRecords: false,
	attendeeRecords: [],
	unknownImages: []
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
        case ACTIONS.RECEIVE_ACTIVE_SESSION:
            return {
				...state,
				currentlyOngoingSession: action.activeSession
            };
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
				attendeeRecords: [],
				unknownImages: []
			};
		case ACTIONS.STOP_REQUEST_ATTENDEE_RECORDS_WITH_ERRORS:
			return {
				...state,
				isLoadingAttendeeRecords: false,
				successfullyLoadedAttendeeRecords: false,
				attendeeRecords: [],
				unknownImages: []
			};
		case ACTIONS.RECEIVE_ATTENDEE_RECORDS_DATA:
			return {
				...state,
				isLoadingAttendeeRecords: false,
				successfullyLoadedAttendeeRecords: true,
				attendeeRecords: action.attendeeRecords
			};
		case ACTIONS.UPDATE_ATTENDEE_RECORD:
			const updateInfo: UpdateRecord = action.updateInfo;
			return {
				...state,
				attendeeRecords: state.attendeeRecords.map(ar =>
					ar.attendee.code === updateInfo.attendeeCode ? ({
						attendee: ar.attendee,
						record: action.updatedRecord
					}) : ar)
			};
		case ACTIONS.UPDATE_ATTENDEE_RECORD_REAL_TIME:
			let updatedRecord: Record | undefined;
			const updatedAttendeeRecord = state.attendeeRecords.find(ar => ar.attendee.code === action.attendeeCode);
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
						present: true,
						image: `${updatedAttendeeRecord.attendee.code}.jpg`
					};
				}
				return {
					...state,
					attendeeRecords: state.attendeeRecords.map(ar =>
						ar.attendee.code === action.attendeeCode ? ({
							attendee: ar.attendee,
							record: updatedRecord
						}) : ar)
				};
			}
		case ACTIONS.START_TAKING_ATTENDANCE:
			return {
				...state,
				currentlyOngoingSession: action.session,
				unknownImages: state.unknownImages
			};
		case ACTIONS.END_TAKING_ATTENDANCE:
			return {
				...state,
				currentlyOngoingSession: undefined,
				unknownImages: state.unknownImages
			};
		case ACTIONS.RECEIVE_UNKNOWN_IMAGES:
			return {
				...state,
				unknownImages: action.unknownImages
			};
		case ACTIONS.UPDATE_UNKNOWN_REAL_TIME:
			return {
				...state,
				unknownImages: [...state.unknownImages, action.image]
			};
		case ACTIONS.REMOVE_UNKNOWN_IMAGE:
			return {
				...state,
				unknownImages: state.unknownImages.filter(img => img !== action.image)
			};
		case ACTIONS.UPDATE_UNKNOWN_REAL_TIME_BATCH:
			return {
				...state,
				unknownImages: [...state.unknownImages, ... action.images]
			};
		case ACTIONS.UPDATE_ATTENDEE_RECORD_REAL_TIME_BATCH:
			const updatedAttendeeRecords = state.attendeeRecords
				.filter(ar => action.attendeeCodes.includes(ar.attendee.code));
			const changes: { [attendeeCode: string]: Record } = {};
			updatedAttendeeRecords.forEach(ar => {
				let updatedRecord: Record | undefined;
				if (ar.record != null) {
					updatedRecord = {
						...ar.record,
						present: true
					};
				} else {
					updatedRecord = {
						id: -1,
						attendee: ar.attendee,
						present: true,
						image: `${ar.attendee.code}.jpg`
					};
				}
				changes[ar.attendee.code] = updatedRecord;
			});
			return {
				...state,
				attendeeRecords: state.attendeeRecords.map(ar =>
					action.attendeeCodes.includes(ar.attendee.code) ? ({
						attendee: ar.attendee,
						record: changes[ar.attendee.code]
					}) : ar)
			};
	}
	return state;
};

export default reducers;
