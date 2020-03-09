import ChangeRequest, { ChangeRequestStatusFilter } from "../../models/ChangeRequest";

export interface ChangeRequestState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    changeRequests: ChangeRequest[];
    unresolvedCount: number;
    filterStatus: ChangeRequestStatusFilter;
};
