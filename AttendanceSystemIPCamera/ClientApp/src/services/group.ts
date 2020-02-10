import ApiResponse from "../models/ApiResponse";
import GroupSearch from "../models/GroupSearch";
import axios from 'axios';

const baseRoute = 'api/group';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getGroups = async (groupSearch: GroupSearch): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute, {
        params: groupSearch
    });
    return await response.data;
};