import ChangeRequest from "../../models/ChangeRequest";

export interface ChangeRequestState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    changeRequests: ChangeRequest[]
};
