import Group from "./Group";
import Attendee from "./Attendee";
import Record from "./Record";

export default interface Classroom {
    id: number;
    name: string;
    rtspString: string;
}