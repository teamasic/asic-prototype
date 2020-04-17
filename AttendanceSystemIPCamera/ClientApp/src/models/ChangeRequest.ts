export enum ChangeRequestStatus {
    UNRESOLVED = 0,
    APPROVED = 1,
    REJECTED = 2
};

export enum ChangeRequestStatusFilter {
    UNRESOLVED = 0,
    RESOLVED = 1,
    ALL = 2
}

export default interface ChangeRequest {
    // record: Record;
    recordId: number;
    attendeeCode: string;
    attendeeName: string;
    groupId: number;
    groupName: string;
    groupCode: string;
    sessionId: number;
    sessionTime: Date;
    sessionName: string;
    comment: string;
    status: ChangeRequestStatus;
    dateSubmitted: Date;
};