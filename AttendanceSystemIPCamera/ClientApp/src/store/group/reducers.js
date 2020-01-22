"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
Object.defineProperty(exports, "__esModule", { value: true });
var actionCreators_1 = require("./actionCreators");
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
var unloadedState = {
    groups: [],
    isLoading: false,
    successfullyLoaded: false
};
var reducers = function (state, incomingAction) {
    if (state === undefined) {
        return unloadedState;
    }
    var action = incomingAction;
    switch (action.type) {
        case actionCreators_1.ACTIONS.START_REQUEST_GROUPS:
            return __assign(__assign({}, state), { isLoading: true, successfullyLoaded: false });
        case actionCreators_1.ACTIONS.STOP_REQUEST_GROUPS_WITH_ERRORS:
            return {
                groups: [],
                isLoading: false,
                successfullyLoaded: false
            };
        case actionCreators_1.ACTIONS.RECEIVE_GROUPS_DATA:
            return {
                groups: action.groups,
                isLoading: false,
                successfullyLoaded: true
            };
    }
    return state;
};
exports.default = reducers;
//# sourceMappingURL=reducers.js.map