import Session from "./Session";
import Attendee from "./Attendee";

export default interface GroupSearch {
    page: number;
    pageSize: number;
    nameContains?: string;
    orderBy?: string;
}