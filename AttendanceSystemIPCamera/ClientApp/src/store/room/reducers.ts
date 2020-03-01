import { Reducer, Action, AnyAction } from "redux";
import { RoomsState } from "./state";
import { ACTIONS } from "./actionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: RoomsState = {
    roomList: [
        {
            id: 0,
            name: 'None',
            rtspString: 'None'
        }
    ],
};

const reducers: Reducer<RoomsState> = (state: RoomsState | undefined, incomingAction: AnyAction): RoomsState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.RECEIVE_CLASSROOMS_DATA:
            return {
                ... state,
                roomList: action.roomList
            };
    }

    return state;
};

export default reducers;