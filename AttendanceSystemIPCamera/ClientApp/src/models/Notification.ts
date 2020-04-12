export enum NotificationType {
    SESSION
}

export default interface Notification {
    id: string;
    type: NotificationType;
    data: any;
    timeSent: Date;
    read: boolean;
}