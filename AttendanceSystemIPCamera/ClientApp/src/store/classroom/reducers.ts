import { Reducer, Action, AnyAction } from "redux";
import { ClassroomsState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: ClassroomsState = {
    isLoading: false,
    classroomList: [],
};

const reducers: Reducer<ClassroomsState> = (state: ClassroomsState | undefined, incomingAction: AnyAction): ClassroomsState => {
    if (state === undefined) {
        return unloadedState;
    }
    debugger;
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.RECEIVE_CLASSROOMS_DATA:
            return {
                ... state,
                isLoading: true,
                classroomList: action.classroomData
            };
    }

    return state;
};

export default reducers;