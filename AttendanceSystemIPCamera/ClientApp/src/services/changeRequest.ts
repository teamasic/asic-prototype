import ApiResponse from "../models/ApiResponse";
import axios from 'axios';
import { ChangeRequestStatusFilter } from "../models/ChangeRequest";
import CreateChangeRequest from "../models/CreateChangeRequest";

const baseRoute = 'api/changeRequest';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getChangeRequest = async (id: number): Promise<ApiResponse> => {
	const response = await axios(apify(id.toString()));
	return await response.data;
};

export const getChangeRequestList =
	async (status: ChangeRequestStatusFilter): Promise<ApiResponse> => {
	const response = await axios(baseRoute, {
		params: {
			status: status.valueOf()
		}
	});
	return await response.data;
};

export const createChangeRequest =
	async (request: CreateChangeRequest): Promise<ApiResponse> => {
		const response = await axios.post(baseRoute, request);
		return await response.data;
	};

export const processChangeRequest =
	async (changeRequestId: number, approved: boolean): Promise<ApiResponse> => {
		const response = await axios.put(baseRoute, {
			changeRequestId,
			approved
		});
		return await response.data;
	};