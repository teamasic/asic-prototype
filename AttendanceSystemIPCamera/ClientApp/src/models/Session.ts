import Group from "./Group";
import Attendee from "./Attendee";
import Record from "./Record";
import Room from "./Room";

export default interface Session {
    id: number;
    name: string;
    startTime: Date;
    endTime: Date;
    group?: Group;
    attendees?: Attendee[];
    records?: Record[];
    active: boolean;
    groupCode?: string;
    room: Room;
    status: string;
}