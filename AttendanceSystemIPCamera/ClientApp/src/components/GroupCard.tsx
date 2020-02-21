﻿import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import * as moment from 'moment'
import Room from '../models/Room';
import { startSession } from '../services/session';
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Select, InputNumber, Typography, Modal, TimePicker } from 'antd';
import { formatFullDateTimeString } from '../utils';
import classNames from 'classnames';
import { createBrowserHistory } from 'history';
import { resolve } from 'dns';
import { rejects } from 'assert';

const { Title } = Typography;
const { Option } = Select;
const { confirm } = Modal;

interface Props {
    roomList: Room[];
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
        startTime: moment(),
        duration: 30,
        roomName: "",
        rtspString: "",
    };

    private takeAttendance = () => {
        this.setState({
            modelOpen: true,
            startTime: moment()
        })
    };

    private handleModelOk = async () => {
        this.setState({
            modelOpen: false,
        })
        const { startTime, duration, roomName, rtspString } = { ...this.state };
        const groupId = this.props.group.id;
        let startTimeString = startTime.format('YYYY-MM-DD HH:mm');
        const data = await startSession({ startTime: startTimeString, duration, rtspString, roomName, groupId });
        if (data != null && data.data != null) {
            const sessionId = data.data.id;
            if (sessionId != null) {
                this.props.redirect(`session/${sessionId}`);
            }
        }
    }

    private handleModelCancel = () => {
        this.setState({
            modelOpen: false,
        })
    }

    private handleChangeStartTime = (time: moment.Moment, timeString: any) => {
        this.setState({
            startTime: time,
        })
    }

    private handleChangeDuration = (duration: number | undefined) => {
        if (duration) {
            this.setState({
                duration: duration
            })
        }
        else {
            this.setState({
                duration: 30
            })
        }
    }
    private onChange = (value: any) => {
        let currentRoom = this.props.roomList.filter(c => c.id == value)[0];
        this.setState({
            roomName: currentRoom.name,
            rtspString: currentRoom.rtspString
        })
    }
    private renderRoomOptions = () => {
        const { roomList } = this.props;
        const roomOptions = roomList.map(room => {
            return <Option key={room.id} value={room.id}>{room.name}</Option>
        })
        return roomOptions;
    }
    private getDisableHours = () => {
        let hours = [];
        for (var i = 0; i < moment().hour(); i++) {
            hours.push(i);
        }
        return hours;
    }
    private getDisableMinutes = () => {
        let minutes = []
        let currentHour = moment().hour();
        let startTimeHour = this.state.startTime.hour();
        if (currentHour == startTimeHour){
            let currentMinute = moment().minute();
            for (let i = 0; i < currentMinute; i++){
                minutes.push(i);
            }
        }
        return minutes;
    }

    private showConfirm = () => {
        confirm({
            title: 'Do you want to delete group ' + this.props.group.name + ' ?',
            onOk() {
                return new Promise((resolve, rejects) => {
                    setTimeout(Math.random() > 0.5 ? resolve : rejects, 1000);
                }).catch(() => { console.log("Oops. Error when deleting group") });
            },
            onCancel() {}
        });
    }

    public render() {
        var group = this.props.group;
        const { startTime, duration } = { ...this.state };
        const endTime = moment(startTime).add(duration, "minutes");
        const lastSessionTime =
			group.sessions.length > 0
				? group.sessions[group.sessions.length - 1].startTime
				: null;
        const menu = (
            <Menu onClick={(click: any) => console.log(click)}>
                <Menu.Item key="1">
                    Edit
                </Menu.Item>
                <Menu.Item key="2" onClick={this.showConfirm}>
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
                    <Button className="take-attendance-button" type="primary" onClick={this.takeAttendance}>Take attendance</Button>
                    <Modal
                        title="Input session for attendance"
                        visible={this.state.modelOpen}
                        onOk={this.handleModelOk}
                        onCancel={this.handleModelCancel}
                        okText="Start"
                    >
                        <div>
                            <Row type="flex" justify="start" align="middle" gutter={[16, 16]}>
                                <Col span={4}>Start time</Col>
                                <Col span={7}><TimePicker disabledMinutes={this.getDisableMinutes} disabledHours={this.getDisableHours} onChange={this.handleChangeStartTime} value={startTime} format={'HH:mm'} /></Col>
                            </Row>
                            <Row type="flex" justify="start" align="middle" gutter={[16, 16]}>
                                <Col span={4}>Duration</Col>
                                <Col><InputNumber min={1} max={90} value={duration} onChange={this.handleChangeDuration} /></Col>
                            </Row>
                            <Row type="flex" justify="start" align="middle" gutter={[16, 16]}>
                                <Col span={4}>End time</Col>
                                <Col span={7}><TimePicker value={endTime} placeholder="" disabled format={'HH:mm'} /></Col>
                            </Row>
                            <Row type="flex" justify="start" align="middle" gutter={[16, 16]}>
                                <Col span={4}>Choose room</Col>
                                <Col span={7}>
                                    <Select
                                        showSearch
                                        style={{ width: 130 }}
                                        placeholder="Select room"
                                        optionFilterProp="children"
                                        onChange={this.onChange}
                                        filterOption={(input: any, option: any) =>
                                            option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
                                        }
                                    >
                                        {this.renderRoomOptions()}
                                    </Select>,
                                </Col>
                            </Row>
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
