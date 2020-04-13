import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Unit from '../models/Unit';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import Room from '../models/Room';
import { createSession } from '../services/session';
import { formatFullDateTimeString, formatDateString, success, error } from '../utils';
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Typography, Modal, Tooltip } from 'antd';
import StartSessionModal from './StartSessionModal';

const { Title } = Typography;
const { confirm } = Modal;

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
            const groupCode = this.props.group.code;
            let currentSession = this.props.units[sessionIndex];
            const data = await createSession({
                groupCode,
                roomId,
                startTime: currentSession.startTime,
                endTime: currentSession.endTime,
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

    private startDeactiveGroup = () => {
        this.props.startDeactiveGroup
            (this.props.group.code, this.props.groupSearch, this.successDeactive, error);
    }

    private successDeactive = () => {
        success("Delete group " + this.props.group.name + " success!");
        Modal.destroyAll();
    }

    private showConfirm = () => {
        confirm({
            title: "Do you want to delete group " + this.props.group.name + " ?",
            okType: "danger",
            okButtonProps: {
                onClick: this.startDeactiveGroup
            },
            onCancel() { }
        });
    }

    public render() {
        var group = this.props.group;
        const lastSessionTime =
            group.sessions.length > 0
                ? group.sessions[group.sessions.length - 1].startTime
                : null;
        const menu = (
            <Menu onClick={(click: any) => console.log(click)}>
                <Menu.Item key="1" onClick={this.showConfirm}>
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
                        <Tooltip title="Delete">
                            <Icon type="delete" onClick={this.showConfirm} />
                        </Tooltip>
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
                    <Button className="past-button" type="link" onClick={this.props.viewDetail} id={group.code}>View Detail</Button>
                    <Button className="take-attendance-button" type="primary" onClick={this.takeAttendance}>New session</Button>
                    <StartSessionModal
                        group={group}
                        hideModal={() => this.handleModelCancel()}
                        modelOpen={this.state.modelOpen}
                        redirect={this.props.redirect}
                        roomList={this.props.roomList}
                        units={this.props.units}
                    />
                </div>
            </Card>
        );
    }
    private formatLastSessionTime(time: Date | null): string {
        if (time != null) {
            return formatDateString(time);
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
