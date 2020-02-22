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
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Select, InputNumber, Typography, Modal, TimePicker } from 'antd';
import { formatFullDateTimeString } from '../utils';
import classNames from 'classnames';
import { createBrowserHistory } from 'history';

const { Title } = Typography;
const { Option } = Select;

interface Props {
    roomList: Room[];
    units: Unit[];
    group: Group;
    viewDetail: any
    redirect: (url: string) => void;
}

// At runtime, Redux will merge together...
type GroupProps = Props &
    GroupsState & // ... state we've requested from the Redux store
    typeof groupActionCreators & // ... plus action creators we've requested
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class GroupCard extends React.PureComponent<GroupProps> {
    public state = {
        modelOpen: false,
        sessionIndex: -1,
        roomId: -1,
        isError: false
    };

    private takeAttendance = () => {
        this.setState({
            modelOpen: true,
            isError: false, 
        })
        
    };

    private handleModelOk = async () => {
        
        const { roomId, sessionIndex } = { ...this.state };
        if (roomId == -1 || sessionIndex == -1) {
            this.setState({
                isError: true
            })
        }
        else {
            this.setState({
                modelOpen: false,
            })
            const groupId = this.props.group.id;
            let currentRoom = this.props.roomList.filter(r => r.id == roomId)[0];
            let currentSession = this.props.units[sessionIndex];
            const abc = {
                startTime: currentSession.startTime,
                endTime: currentSession.endTime,
                rtspString: currentRoom.rtspString,
                roomName: currentRoom.name,
                groupId,
                name: currentSession.name
            };
            console.log(abc);
            const data = await createSession({
                startTime: currentSession.startTime,
                endTime: currentSession.endTime,
                rtspString: currentRoom.rtspString,
                roomName: currentRoom.name,
                groupId,
                name: currentSession.name
            });
            if (data != null && data.data != null) {
                const sessionId = data.data.id;
                if (sessionId != null) {
                    this.props.redirect(`session/${sessionId}`);
                }
            }
        }

    }

    private handleModelCancel = () => {
        this.setState({
            modelOpen: false,
        })
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
        var group = this.props.group;
        const lastSessionTime =
            group.sessions.length > 0
                ? group.sessions[group.sessions.length - 1].startTime
                : null;
        const menu = (
            <Menu onClick={(click: any) => console.log(click)}>
                <Menu.Item key="1">
                    Edit
                </Menu.Item>
                <Menu.Item key="2">
                    Delete
                </Menu.Item>
            </Menu>
        );
        return (
            <Card className="group shadow">
                <Row>
                    <Col span={22}>
                        <Title level={4}>{group.name}</Title>
                    </Col>
                    <Col span={2}>
                        <Dropdown overlay={menu}>
                            <Button icon="ellipsis" type="link"></Button>
                        </Dropdown>
                    </Col>
                </Row>
                <div className="description-container">
                    <div className="description">
                        <Icon type="user" /><span>{group.attendees.length} {group.attendees.length > 1 ? 'attendees' : 'attendee'}</span>
                    </div>
                    <div className="description">
                        <Icon type="calendar" /><span>{group.sessions.length} {group.sessions.length > 1 ? 'sessions' : 'session'}</span>
                    </div>
                    <div className="description">
                        <Icon type="history" />
                        <span>
                            Last session: {this.formatLastSessionTime(lastSessionTime)}
                        </span>
                    </div>
                </div>
                <div className="actions">
                    <Button className="past-button" type="link" onClick={this.props.viewDetail} id={group.id.toString()}>View Detail</Button>
                    <Button className="take-attendance-button" type="primary" onClick={this.takeAttendance}>New session</Button>
                    <Modal
                        title="Create new session"
                        visible={this.state.modelOpen}
                        onOk={this.handleModelOk}
                        onCancel={this.handleModelCancel}
                        okText="Start"
                    >
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
                                    </Select>,
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
                                    </Select>,
                                </Col>
                            </Row>
                            {this.state.isError ? <Row type="flex" justify="start" align="top" gutter={[16, 16]}>
                                <Col span={14}><p style={{ color: "red" }}>Please choose room and unit</p></Col>
                            </Row> : null}

                        </div>
                    </Modal>
                </div>
            </Card>
        );
    }
    private formatLastSessionTime(time: Date | null): string {
        if (time != null) {
            return formatFullDateTimeString(time);
        }
        return 'Never';
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.groups,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(GroupCard as any);
