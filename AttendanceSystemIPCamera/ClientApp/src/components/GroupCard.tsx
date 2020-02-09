import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Card, Button, Dropdown, Icon, Menu, Row, Col } from 'antd';
import { Typography } from 'antd';
import classNames from 'classnames';

const { Title } = Typography;


interface Props {
    group: Group;
}

// At runtime, Redux will merge together...
type GroupProps =
    Props &
    GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters


class GroupCard extends React.PureComponent<GroupProps> {
    public render() {
        var group = this.props.group;
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
                    <Button className="take-attendance-button" type="primary">Take attendance</Button>
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
