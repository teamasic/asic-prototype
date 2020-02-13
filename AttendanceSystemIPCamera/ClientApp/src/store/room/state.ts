import Group from "../../models/Group";
import PaginatedList from "../../models/PaginatedList";
import GroupSearch from "../../models/GroupSearch";
import Session from "../../models/Session";
import Room from "../../models/Room";

export interface RoomsState {
    roomList: Room[];
}
