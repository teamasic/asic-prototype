import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/schedule';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getSchedules = async (): Promise<ApiResponse> => {
	const response = await axios(baseRoute);
	return await response.data;
};