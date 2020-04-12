import ApiResponse from "../models/ApiResponse";
import axios from 'axios';
import ScheduleCreate from "../models/ScheduleCreate";

const baseRoute = 'api/schedule';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getSchedules = async (): Promise<ApiResponse> => {
	const response = await axios(baseRoute);
	return await response.data;
};

export const createSchedules = async (schedules: ScheduleCreate[]): Promise<ApiResponse> => {
	const response = await axios.post(baseRoute, schedules);
	return await response.data;
}

export const getByGroupId = async (groupId: number): Promise<ApiResponse> => {
	const response = await axios.get(apify("group"), {
		params: {
			groupId: groupId
		}
	});
	return await response.data;
}