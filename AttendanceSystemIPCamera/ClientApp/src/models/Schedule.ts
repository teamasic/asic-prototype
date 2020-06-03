export default interface Schedule {
    id: number;
    groupId: number;
    slot: string;
    room: string;
    startTime: Date;
    endTime: Date;
}