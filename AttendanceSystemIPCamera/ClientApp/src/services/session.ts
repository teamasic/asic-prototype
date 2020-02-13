import ApiResponse from '../models/ApiResponse';

const baseRoute = 'api/session';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getSession = async (id: number): Promise<ApiResponse> => {
	const response = await fetch(apify(id.toString()));
	return await response.json();
};

export const getSessionAttendeeRecordList = async (
	id: number
): Promise<ApiResponse> => {
	const response = await fetch(apify(`${id.toString()}/attendee-records`));
	return await response.json();
};
