import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/settings';
const apify = (path: string) => `${baseRoute}/${path}`;
const apifyServer = (path: string) => `${process.env.SERVER_URL}/${baseRoute}/${path}`;

export const checkForUpdates = async (): Promise<ApiResponse> => {
	const response = await axios(baseRoute);
	return await response.data;
};