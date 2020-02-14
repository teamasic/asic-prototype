import ApiResponse from "../models/ApiResponse";
import GroupSearch from "../models/GroupSearch";
import Group from "../models/Group";
import axios from 'axios';

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