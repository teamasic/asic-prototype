import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Unit from '../models/Unit';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import * as moment from 'moment'
import Room from '../models/Room';
import { createSession } from '../services/session';
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Select, InputNumber, Typography, Modal, TimePicker, message } from 'antd';
import { formatFullDateTimeString, success, error } from '../utils';
import classNames from 'classnames';
import { createBrowserHistory } from 'history';
import SessionStatusConstants, { SessionStatusText } from "../constants/SessionStatusConstants";

const { Title } = Typography;
const { Option } = Select;
const { confirm } = Modal;


interface Props {
    modelOpen: boolean;
    hideModal: () => void;
    roomList: Room[];
    units: Unit[];
    group: Group;
    redirect: (url: string) => void;
}

// At runtime, Redux will merge together...
type StartSessionModalProps = Props &
    GroupsState & // ... state we've requested from the Redux store
    typeof groupActionCreators & // ... plus action creators we've requested
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class StartSessionModal extends React.PureComponent<StartSessionModalProps> {

    public state = {
        sessionIndex: -1,
        roomId: -1,
        isError: false,
    };

    private handleModelOk = async () => {
        const { roomId, sessionIndex } = { ...this.state };
        if (roomId == -1 || sessionIndex == -1) {
           error("Please choose room and unit")
        }
        else {
            const groupCode = this.props.group.code;
            const currentSession = this.props.units[sessionIndex];
            const startTime = currentSession.startTime;
            const endTime = currentSession.endTime;
            const name = currentSession.name;
            const data = await createSession({
                groupCode,
                roomId,
                startTime,
                endTime,
                name
            });
            if (data != null && data.data != null) {
                this.handleModelCancel();
                const sessionId = data.data.id;
                this.setState({
                    isError: false
                });
                success(`Session is created for ${groupCode} with status ${(SessionStatusText as any)[data.data.status]}`)
                if (data.data.status == SessionStatusConstants.SCHEDULED){
                    window.location.href = `/group/${groupCode}?tab=2`
                }
                else {
                    this.props.redirect(`/session/${sessionId}`);
                }
            }
            else {
                const startTimeDate = new Date(startTime);
                let errorMessage = `Session is already created for ${groupCode} at ${currentSession.name} ${startTimeDate.getDate()}/${startTimeDate.getMonth() + 1}/${startTimeDate.getFullYear()}`;
                error(errorMessage)
            }
        }

    }

    private handleModelCancel = () => {
        this.props.hideModal();
    }

    private onChangeRoom = (value: any) => {
        this.setState({
            roomId: value
        })
    }
    private onChangeUnit = (value: any) => {
        this.setState({
            sessionIndex: value
        })
    }
    private renderRoomOptions = () => {
        const { roomList } = this.props;
        const roomOptions = roomList.map((room, index) => {
            return <Option key={room.id} value={room.id}>{room.name}</Option>
        })
        return roomOptions;
    }
    private renderUnitOptions = () => {
        const roomOptions = this.props.units.map((unit, index) => {
            return <Option key={unit.name} value={index}>{unit.name}</Option>
        })
        return roomOptions;
    }

    public render() {
        return <Modal
            title="Create new session"
            visible={this.props.modelOpen}
            onOk={this.handleModelOk}
            onCancel={this.handleModelCancel}
            okText="Start">
            <div>
                <Row type="flex" justify="start" align="top" gutter={[16, 16]}>
                    <Col span={6}><p>Choose room</p></Col>
                    <Col span={7}>
                        <Select
                            showSearch
                            style={{ width: 130 }}
                            placeholder="Select room"
                            optionFilterProp="children"
                            onChange={this.onChangeRoom}
                            filterOption={(input: any, option: any) =>
                                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
                            }
                        >
                            {this.renderRoomOptions()}
                        </Select>
                    </Col>
                </Row>
                <Row type="flex" justify="start" align="top" gutter={[16, 16]}>
                    <Col span={6}>Choose unit</Col>
                    <Col span={7}>
                        <Select
                            showSearch
                            style={{ width: 130 }}
                            placeholder="Select unit"
                            optionFilterProp="children"
                            onChange={this.onChangeUnit}
                            filterOption={(input: any, option: any) =>
                                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
                            }
                        >
                            {this.renderUnitOptions()}
                        </Select>
                    </Col>
                </Row>
                {this.state.isError ? <Row type="flex" justify="start" align="top" gutter={[16, 16]}>
                    <Col span={14}><p style={{ color: "red" }}>Please choose room and unit</p></Col>
                </Row> : null}

            </div>
        </Modal>;
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.groups,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(StartSessionModal as any);
