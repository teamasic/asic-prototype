import { Reducer, Action, AnyAction } from "redux";
import { UnitsState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: UnitsState = {
    units: [],
};

const reducers: Reducer<UnitsState> = (state: UnitsState | undefined, incomingAction: AnyAction): UnitsState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.RECEIVE_UNITS_DATA:
            return {
                ... state,
                units: action.units
            };
    }

    return state;
};

export default reducers;