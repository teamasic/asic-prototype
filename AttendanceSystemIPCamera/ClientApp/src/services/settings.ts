import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/settings';
const apify = (path: string) => `${baseRoute}/${path}`;

export const checkForUpdates = async (): Promise<ApiResponse> => {
	const response = await axios(baseRoute);
	return await response.data;
};

export const update = async (key: string): Promise<ApiResponse> => {
	const response = await axios.post(apify(key));
	return await response.data;
};