import Attendee from "./Attendee";

export default interface Record {
    id: number;
    attendee: Attendee;
    present: boolean;
}