export enum NotificationType {
    SESSION
}

export default interface Notification {
    type: NotificationType;
    data: any;
    timeSent: Date;
}