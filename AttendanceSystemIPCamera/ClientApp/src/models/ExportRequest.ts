export default interface ExportRequest {
    groupId: number,
    isSingleDate: boolean,
    withCondition: boolean,
    singleDate: Date,
    startDate: Date, 
    endDate: Date,
    isPresent: boolean,
    isGreaterThanOrEqual: boolean,
    attendancePercent: number
}