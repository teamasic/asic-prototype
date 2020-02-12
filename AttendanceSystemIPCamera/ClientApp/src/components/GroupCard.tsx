import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Select, InputNumber } from 'antd';
import { Typography, Modal, TimePicker } from 'antd';
import * as moment from 'moment'
import Classroom from '../models/classroom';
import {startSession} from '../services/session';
const { Title } = Typography;
const { Option } = Select;


interface Props {
    group: Group;
    classroomList: Classroom[];
}
interface CurrentState {
    modelOpen: boolean,
    startTime: moment.Moment,
    classroom: Classroom,
    duration: number
}

// At runtime, Redux will merge together...
type GroupProps =
    Props
    & CurrentState
    & GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters


class GroupCard extends React.PureComponent<GroupProps> {
    public state = {
        modelOpen: false,
        startTime: moment(),
        duration: 30,
        classroomName: "",
        rtspString: "",
    };

    private takeAttendance = () => {
        this.setState({
            modelOpen: true,
            startTime: moment()
        })
    };

    private handleModelOk = () => {
        this.setState({
            modelOpen: false,
        })
        const {startTime, duration, classroomName, rtspString} = {...this.state};
        const groupId = this.props.group.id;
        let startTimeString = startTime.format('YYYY-MM-DD HH:mm');
        console.log({startTime: startTimeString, duration, rtspString, classroomName, groupId});
        startSession({startTime: startTimeString, duration, rtspString, classroomName, groupId});
    }

    private handleModelCancel = () => {
        this.setState({
            modelOpen: false,
        })
    }

    private handleChangeStartTime = (time: any, timeString: any) => {
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
        let currentClassroom = this.props.classroomList.filter(c => c.id == value)[0];
        this.setState({
            classroomName: currentClassroom.name,
            rtspString: currentClassroom.rtspString
        })
    }
    private renderClassroomOptions = () => {
        const { classroomList } = this.props;
        console.log(classroomList);
        const classroomOptions = classroomList.map(classroom => {
            return <Option key={classroom.id} value={classroom.id}>{classroom.name}</Option>
        })
        return classroomOptions;
    }
    public render() {
        var group = this.props.group;
        const { startTime, duration } = { ...this.state };
        const endTime = moment(startTime).add(duration, "minutes");
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
                        <Icon type="history" /><span>Last session: Today</span>
                    </div>
                </div>
                <div className="actions">
                    <Button className="past-button" type="link">Past sessions</Button>
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
                                <Col span={7}><TimePicker onChange={this.handleChangeStartTime} value={startTime} format={'HH:mm'} /></Col>
                            </Row>
                            <Row type="flex" justify="start" align="middle" gutter={[16, 16]}>
                                <Col span={4}>Duration</Col>
                                <Col><InputNumber min={5} max={90} value={duration} onChange={this.handleChangeDuration} /></Col>
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
                                        style={{ width: 200 }}
                                        placeholder="Select a person"
                                        optionFilterProp="children"
                                        onChange={this.onChange}
                                        filterOption={(input: any, option: any) =>
                                            option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
                                        }
                                    >
                                        {this.renderClassroomOptions()}
                                    </Select>,
                                </Col>
                            </Row>
                        </div>
                    </Modal>
                </div>
            </Card>
        );
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.groups,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(GroupCard as any);
