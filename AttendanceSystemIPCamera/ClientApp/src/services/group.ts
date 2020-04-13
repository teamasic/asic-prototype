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

export const createAttendeeInGroup = async (code: string, attendee: Attendee): Promise<ApiResponse> => {
    var url = baseRoute + '/' + code + '/attendee';
    const response = await axios.post(url, attendee);
    return await response.data;
}

export const getGroupDetail = async (code: string): Promise<ApiResponse> => {
    var url = baseRoute + '/' + code;
    const response = await axios.get(url);
    return await response.data;
};

export const deactiveGroup = async (code: string): Promise<ApiResponse> => {
    var url = baseRoute + '/deactive/' + code;
    const response = await axios.put(url);
    return await response.data;
};


export const updateGroup = async (code: string, updatedGroup: Group): Promise<ApiResponse> => {
    var url = baseRoute + '/' + code;
    const response = await axios.put(url, updatedGroup);
    return await response.data;
};

export const deleteAttendeeGroup = async (attendeeCode: string, groupCode: string): Promise<ApiResponse> => {
    var url = baseRoute + '/' + groupCode;
    const response = await axios.delete(url, {
        params: {
            attendeeCode
        }
    });
    return await response.data;
}