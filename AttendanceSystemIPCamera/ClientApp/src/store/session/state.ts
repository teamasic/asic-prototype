import Group from '../../models/Group';
import PaginatedList from '../../models/PaginatedList';
import GroupSearch from '../../models/GroupSearch';
import Session from '../../models/Session';
import AttendeeRecordPair from '../../models/AttendeeRecordPair';
import Room from "../../models/Room";

export interface SessionState {
	isLoadingSession: boolean;
	successfullyLoadedSession: boolean;
	activeSession?: Session;
	currentlyOngoingSession?: Session;
	isLoadingAttendeeRecords: boolean;
	successfullyLoadedAttendeeRecords: boolean;
	attendeeRecords: AttendeeRecordPair[];
}