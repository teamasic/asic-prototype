import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/classroom';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getClassrooms = async (): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute);
    return await response.data;
};