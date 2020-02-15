import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/session';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getGroups = async (): Promise<ApiResponse> => {
    const response = await fetch(baseRoute);
    return await response.json();
};

export const startSession = (data: any) => {
     axios.post(baseRoute, data)
}
export const getActiveSession = async (): Promise<ApiResponse> => {
    const response = await fetch(apify("active"));
    return await response.json();
}