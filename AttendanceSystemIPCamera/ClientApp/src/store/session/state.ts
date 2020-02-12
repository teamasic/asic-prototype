import Group from "../../models/Group";
import PaginatedList from "../../models/PaginatedList";
import GroupSearch from "../../models/GroupSearch";
import Session from "../../models/Session";

export interface GroupsState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    paginatedGroupList?: PaginatedList<Group>;
    groupSearch: GroupSearch;
    activeSession?: Session;
}
