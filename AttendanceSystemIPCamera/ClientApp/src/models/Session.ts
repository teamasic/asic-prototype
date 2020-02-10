import Group from "./Group";
import Attendee from "./Attendee";
import Record from "./Record";

export default interface Session {
    id: number;
    startTime: Date;
    group?: Group;
    attendees?: Attendee[];
    records?: Record[];
}