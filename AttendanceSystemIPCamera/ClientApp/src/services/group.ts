import ApiResponse from "../models/ApiResponse";

const baseRoute = 'api/group';
const apify = (path: string) => `${baseRoute}/${path}`;

export const getGroups = async (): Promise<ApiResponse> => {
    const response = await fetch(baseRoute);
    return await response.json();
};