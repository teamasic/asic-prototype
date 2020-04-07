import { ExportMultipleCondition } from "./ExportMultipleCondition";

export default interface ExportRequest {
    groupId: number,
    isSingleDate: boolean,
    withCondition: boolean,
    singleDate: string,
    startDate: string, 
    endDate: string,
    isPresent: boolean,
    multipleDateCondition: ExportMultipleCondition,
    attendancePercent: number
}