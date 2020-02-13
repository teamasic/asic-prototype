import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/room';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getRooms = async (): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute);
    return await response.data;
};