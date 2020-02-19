import Attendee from './Attendee';
import Record from './Record';

export default interface AttendeeRecordPair {
	attendee: Attendee;
	record?: Record;
}
