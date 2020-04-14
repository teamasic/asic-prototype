import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import UserLogin from "../../models/UserLogin";
import { constants } from "../../constant";
import { UserLoginResponse } from "../../models/UserLoginResponse";
import { loginWithFirebase } from "../../services/User";

export const ACTIONS = {
    START_REQUEST_LOGIN: "START_REQUEST_LOGIN",
    STOP_REQUEST_LOGIN_WITH_ERRORS: "STOP_REQUEST_LOGIN_WITH_ERRORS",
    RECEIVE_SUCCESS_LOGIN: "RECEIVE_SUCCESS_LOGIN",
    USER_INFO_NOT_IN_LOCAL: "USER_INFO_NOT_IN_LOCAL",
    LOG_OUT: "LOG_OUT"
}

function startRequestLogin() {
    return {
        type: ACTIONS.START_REQUEST_LOGIN
    };
}

function stopRequestLoginWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_LOGIN_WITH_ERRORS,
        errors
    };
}

function receiveSuccessLogin(userLoginResponse: UserLoginResponse) {
    return {
        type: ACTIONS.RECEIVE_SUCCESS_LOGIN,
        user: userLoginResponse
    };
}

function checkUserInfo() {
    const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
    if (authData) {
        const user = JSON.parse(authData);
        return {
            type: ACTIONS.RECEIVE_SUCCESS_LOGIN,
            user
        }
    }
    return {
        type: ACTIONS.USER_INFO_NOT_IN_LOCAL
    }
}

function logout(){
    return {
        type: ACTIONS.LOG_OUT
    }
}

const requestLogin = (userLogin: UserLogin, redirect: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestLogin());

    const apiResponse: ApiResponse = await loginWithFirebase(userLogin);
    if (apiResponse.success) {
        console.log(apiResponse.data);
        localStorage.setItem(constants.AUTH_IN_LOCAL_STORAGE, JSON.stringify(apiResponse.data));
        dispatch(receiveSuccessLogin(apiResponse.data));
        redirect();
    } else {
        dispatch(stopRequestLoginWithError(apiResponse.errors));
    }
}

export const userActionCreators = {
    requestLogin: requestLogin,
    checkUserInfo: checkUserInfo,
    logout: logout,
};