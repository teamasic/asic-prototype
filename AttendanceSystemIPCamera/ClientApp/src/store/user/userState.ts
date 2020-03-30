import User from "../../models/User";

export interface UserState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    currentUser: User;
}