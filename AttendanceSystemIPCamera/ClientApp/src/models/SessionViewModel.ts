export default interface SessionViewModel {
    id: number;
    name: string;
    startTime: string;
    endTime: string;
}

export interface SessionNotificationViewModel {
    id: number;
    name: string;
    startTime: string;
    endTime: string;
    groupId: number;
    groupName: string;
    roomName: string;
}