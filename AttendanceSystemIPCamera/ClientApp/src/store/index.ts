import { GroupsState } from './group/state';
import { SessionState } from './session/state';
import GroupsReducer from './group/reducers';
import SessionsReducer from './session/reducers';
import UnitsReducer from './unit/reducers';
import RoomsReducer from './room/reducers';
import ChangeRequestsReducer from './changeRequest/reducers';
import { AnyAction } from 'redux';
import { RoomsState } from './room/state';
import { UnitsState } from './unit/state';
import { ChangeRequestState } from './changeRequest/state';

// The top-level state object
export interface ApplicationState {
	groups: GroupsState | undefined;
	sessions: SessionState | undefined;
	rooms: RoomsState | undefined;
	units: UnitsState | undefined;
	changeRequests: ChangeRequestState | undefined;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
	sessions: SessionsReducer,
    rooms: RoomsReducer,
	groups: GroupsReducer,
	units: UnitsReducer,
	changeRequests: ChangeRequestsReducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction {
	(
		dispatch: (action: AnyAction) => void,
		getState: () => ApplicationState
	): void;
}
