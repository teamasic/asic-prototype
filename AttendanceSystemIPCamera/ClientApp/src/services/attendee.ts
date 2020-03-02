import ApiResponse from "../models/ApiResponse";
import Attendee from "../models/Attendee";
import axios from 'axios';

const baseRoute = 'api/attendee';

export const getAttendeeByCode = async (code: string): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute, {
        params: {
            code: code
        }
    });
    return await response.data;
};