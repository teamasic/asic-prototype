import Session from "./Session";
import Attendee from "./Attendee";

export default class Group {
    code: string = "";
    name: string = "";
    sessions: Session[] = [];
    attendees: Attendee[] = [];
    totalSession: number = 0;
}