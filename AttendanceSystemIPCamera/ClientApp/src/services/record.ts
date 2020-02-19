import ApiResponse from '../models/ApiResponse';
import axios from 'axios';
import UpdateRecord from '../models/UpdateRecord';

const baseRoute = 'api/record';
const apify = (path: string) => `${baseRoute}/${path}`;

export const updateRecord = async (updateRecord: UpdateRecord): Promise<ApiResponse> => {
	const response = await axios.post(apify('manually'), updateRecord);
	return response.data;
};