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
    groupCode: string;
    groupName: string;
    roomName: string;
}