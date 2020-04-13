import ApiResponse from "../models/ApiResponse";
import axios from 'axios';
import ExportRequest from "../models/ExportRequest";

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

export const getSessionUnknownImagesList = async (
	id: number
): Promise<ApiResponse> => {
	const response = await axios(apify(`${id.toString()}/unknown`));
	return await response.data;
};

export const createSession = async (data: any) => {
	const response = await axios.post(baseRoute, data)
	return await response.data;
};

export const getActiveSession = async (): Promise<ApiResponse> => {
    const response = await axios(apify("active"));
    return await response.data;
}

export const exportSession = async (exportRequest: any): Promise<ApiResponse> => {
	const response = await axios.post(apify("export"), exportRequest);
	return await response.data;
}

export const takeAttendance = async (data: any) => {
	const response = await axios.post(apify("take-attendance"), data)
	return await response.data;
};

export const getPastSession = async (groupCode: string) => {
	const response = await axios.get(apify("past"), {
		params: {
			groupCode
		}
	});
	return await response.data;
}
