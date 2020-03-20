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
type GroupProps = Props &
    GroupsState & // ... state we've requested from the Redux store
    typeof groupActionCreators & // ... plus action creators we've requested
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class GroupCard extends React.PureComponent<GroupProps> {
    public state = {
        sessionIndex: -1,
        roomId: -1,
        isError: false
    };

    private handleModelOk = async () => {
        const { roomId, sessionIndex } = { ...this.state };
        if (roomId == -1 || sessionIndex == -1) {
            // this.setState({
            //     isError: true
            // })
           error("Please choose room and unit")
        }
        else {
            const groupId = this.props.group.id;
            let currentRoom = this.props.roomList.filter(r => r.id == roomId)[0];
            let currentSession = this.props.units[sessionIndex];
            const data = await createSession({
                startTime: currentSession.startTime,
                endTime: currentSession.endTime,
                rtspString: currentRoom.rtspString,
                roomName: currentRoom.name,
                groupId,
                name: currentSession.name
            });
            if (data != null && data.data != null) {
                this.handleModelCancel();
                const sessionId = data.data.id;
                this.setState({
                    isError: false
                });
                if (sessionId != null) {
                    this.props.redirect(`session/${sessionId}`);
                }
            }
            else {
                error("Session with this unit existed")
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
)(GroupCard as any);
