import Group from "../../models/Group";

export interface GroupsState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    groups: Group[];
}
