import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import Schedule from '../../models/Schedule';
// import { createSchedules, getByGroupId } from "../../services/schedule";
import ScheduleCreate from "../../models/ScheduleCreate";
import { success, error, warning } from "../../utils";

export const ACTIONS = {
};

// export const requestCreateSchedules = (schedules: ScheduleCreate[], reloadData: Function): AppThunkAction => async (dispatch, getState) => {
// 	const apiResponse: ApiResponse = await createSchedules(schedules);
// 	if(apiResponse.success) {
//         var countCreatedItem = apiResponse.data.length;
//         if(countCreatedItem > 0) {
//             success("Create " + countCreatedItem + " schedules successfully!");
//         } else {
//             warning("Data is not valid. Created failed!");
//         }
//         reloadData();
//     } else {
//         error("Oops.. something went wrong!");
//         console.log(apiResponse.errors);
//     }
// }

// export const requestGetByGroupCode = (groupId: string, loadData: Function): AppThunkAction => async (dispatchEvent, getState) => {
//     const apiResponse: ApiResponse = await getByGroupId(groupId);
//     if(apiResponse.success) {
//         loadData(apiResponse.data);
//     } else {
//         error("Oops.. something went wrong!");
//         console.log(apiResponse.errors);
//     }
// }

// export const scheduleActionCreators = {
//     requestCreateSchedules: requestCreateSchedules,
//     requestGetByGroupId: requestGetByGroupCode
// }