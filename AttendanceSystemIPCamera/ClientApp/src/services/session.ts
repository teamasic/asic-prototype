import ApiResponse from "../models/ApiResponse";
import axios from 'axios';

const baseRoute = 'api/session';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getSession = async (id: number): Promise<ApiResponse> => {
	const response = await axios(apify(id.toString()));
	return await response.data;
};

export const getSessionAttendeeRecordList = async (
	id: number
): Promise<ApiResponse> => {
	const response = await axios(apify(`${id.toString()}/attendee-records`));
	return await response.data;
};

export const startSession = async (data: any) => {
	const response = await axios.post(baseRoute, data)
	return await response.data;
};

export const getActiveSession = async (): Promise<ApiResponse> => {
    const response = await axios(apify("active"));
    return await response.data;
}