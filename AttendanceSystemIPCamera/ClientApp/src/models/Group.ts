import Session from "./Session";
import Attendee from "./Attendee";

export default class Group {
    id: number = 0;
    name: string = "";
    sessions: Session[] = [];
    attendees: Attendee[] = [];
    get lastSessionTime(): Date | undefined {
        if (this.sessions.length > 0) {
            return this.sessions[this.sessions.length - 1].startTime;
        }
    }
}