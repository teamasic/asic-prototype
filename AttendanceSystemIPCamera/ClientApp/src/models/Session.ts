import Group from "./Group";
import Attendee from "./Attendee";

export default interface Session {
    id: number;
    startTime: Date;
    group?: Group;
    attendees?: Attendee[];
}