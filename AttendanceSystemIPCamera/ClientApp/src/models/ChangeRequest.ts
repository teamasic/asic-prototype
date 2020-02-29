import Attendee from "./Attendee";
import Record from "./Record";

export enum ChangeRequestStatus {
    UNRESOLVED = 0,
    APPROVED = 1,
    DENIED = 2
};

export enum ChangeRequestStatusFilter {
    UNRESOLVED = 0,
    RESOLVED = 1,
    ALL = 2
}

export default interface ChangeRequest {
    id: number;
    record: Record;
    commment: string;
    oldState: boolean;
    newState: boolean;
    status: ChangeRequestStatus;
};