import Group from "./Group";
import Attendee from "./Attendee";
import Record from "./Record";

export default interface Room {
    id: number;
    name: string;
    rtspString: string;
}