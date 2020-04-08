import { Reducer, Action, AnyAction } from "redux";
import { ScheduleState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: ScheduleState = {
    schedules: [],
};

const reducers: Reducer<ScheduleState> = (state: ScheduleState | undefined,
    incomingAction: AnyAction): ScheduleState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
    }

    return state;
};

export default reducers;