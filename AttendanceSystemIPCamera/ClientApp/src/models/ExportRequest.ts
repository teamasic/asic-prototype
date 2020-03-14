export default interface ExportRequest {
    groupId: number,
    isSingleDate: boolean,
    withCondition: boolean,
    singleDate: string,
    startDate: string, 
    endDate: string,
    isPresent: boolean,
    isGreaterThanOrEqual: boolean,
    attendancePercent: number
}