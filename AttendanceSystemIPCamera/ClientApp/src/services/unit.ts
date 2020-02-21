import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/unit';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getUnits = async (): Promise<ApiResponse> => {
	const response = await axios(baseRoute);
	return await response.data;
};