import Session from "./Session";
import Attendee from "./Attendee";

export default class Group {
    id: number = 0;
    code: string = "";
    name: string = "";
    code: string = "";
    sessions: Session[] = [];
    attendees: Attendee[] = [];
}