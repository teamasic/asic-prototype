import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/session';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getGroups = async (): Promise<ApiResponse> => {
    const response = await fetch(baseRoute);
    return await response.json();
};

export const startSession = (data: any) => {
    const response = axios.post(baseRoute, data)
}