import ApiResponse from "../models/ApiResponse";
import GroupSearch from "../models/GroupSearch";
import Group from "../models/Group";
import axios from 'axios';
import Attendee from "../models/Attendee";

const baseRoute = 'api/group';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getGroups = async (groupSearch: GroupSearch): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute, {
        params: groupSearch
    });
    return await response.data;
};

export const createGroup = async (newGroup: Group): Promise<ApiResponse> => {
    const response = await axios.post(baseRoute, newGroup);
    return await response.data;
};

export const createAttendeeInGroup = async (id: number, attendee: Attendee): Promise<ApiResponse> => {
    var url = baseRoute + '/' + id + '/attendee';
    const response = await axios.post(url, attendee);
    return await response.data;
}

export const getGroupDetail = async (id: number): Promise<ApiResponse> => {
    var url = baseRoute + '/' + id;
    const response = await axios.get(url);
    return await response.data;
};

export const deactiveGroup = async (id: number): Promise<ApiResponse> => {
    var url = baseRoute + '/deactive/' + id;
    const response = await axios.put(url);
    return await response.data;
};


export const updateGroup = async (id: number, newName: string): Promise<ApiResponse> => {
    var url = baseRoute + '/' + id + '?groupName=' + newName;
    const response = await axios.put(url);
    return await response.data;
};

export const deleteAttendeeGroup = async (attendeeId: number, groupId: number): Promise<ApiResponse> => {
    var url = baseRoute + '/' + groupId;
    const response = await axios.delete(url, {
        params: {
            attendeeId: attendeeId
        }
    });
    return await response.data;
}