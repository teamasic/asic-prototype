import Session from "./Session";
import Attendee from "./Attendee";

export default class Group {
    id: number = 0;
    name: string = "";
    sessions: Session[] = [];
    attendees: Attendee[] = [];
}