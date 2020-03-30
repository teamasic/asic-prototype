import ApiResponse from "../models/ApiResponse";
import axios from "axios";
import UserLogin from "../models/UserLogin";
import { constants } from "../constant";

const baseRoute  = constants.BASE_ROUTE + "user";
const apify = (path: string) => `${baseRoute}/${path}`;

export const loginWithFirebase = async (userLogin: UserLogin): Promise<ApiResponse> => {
    const response = await axios.post(apify('login/firebase'), userLogin);
    return await response.data;
};



