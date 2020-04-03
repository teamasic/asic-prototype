import Setting from "../../models/Setting";

export interface SettingState {
    room: Setting;
    unit: Setting;
    model: Setting;
    others: Setting;
}
