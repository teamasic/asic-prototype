import Group from "../../models/Group";
import PaginatedList from "../../models/PaginatedList";
import GroupSearch from "../../models/GroupSearch";
import Session from "../../models/Session";
import Classroom from "../../models/classroom";

export interface ClassroomsState {
    isLoading: boolean;
    classroomList: Classroom[];
}
