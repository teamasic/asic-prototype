import Group from "./Group";
import Attendee from "./Attendee";
import Record from "./Record";

export default interface Session {
    id: number;
    name: string;
    startTime: Date;
    endTime: Date;
    group?: Group;
    attendees?: Attendee[];
    records?: Record[];
    active: boolean;
}