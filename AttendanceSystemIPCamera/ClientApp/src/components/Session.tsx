import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Breadcrumb, Icon, Button, Empty, Select, List, Card, Spin, Row, Col, Table, Tag } from 'antd';
import { Typography } from 'antd';
import { Input } from 'antd';
import classNames from 'classnames';
import '../styles/Session.css';

const { Search } = Input;
const { Title } = Typography;

// At runtime, Redux will merge together...
type SessionProps =
    GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters


class Session extends React.PureComponent<SessionProps> {
    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public searchBy(query: string) {

    }

    public render() {
        const columns = [
            {
                title: 'Id',
                dataIndex: 'id',
                key: 'id',
            },
            {
                title: 'Name',
                dataIndex: 'name',
                key: 'name',
            },
            {
                title: 'Status',
                key: 'status',
                render: (text: string, attendee: Attendee) => (
                    <span>Present</span>
                ),
            },
            {
                title: 'Actions',
                key: 'present',
                render: (text: string, attendee: Attendee) => (
                    <div>
                        <Button type="primary">Present</Button>
                        <Button type="danger">Absent</Button>
                    </div>
                ),
            },
        ];
        return (
            <React.Fragment>
                <div className="breadcrumb-container">
                    <Breadcrumb>
                        <Breadcrumb.Item href="">
                            <Icon type="home" />
                        </Breadcrumb.Item>
                        <Breadcrumb.Item>
                            <Icon type="hdd" />
                            <span>Group</span>
                        </Breadcrumb.Item>
                        <Breadcrumb.Item>
                            <Icon type="calendar" />
                            <span>Session</span>
                        </Breadcrumb.Item>
                    </Breadcrumb>
                </div>
                <div className="title-container">
                    <Title className="title" level={3}>Session</Title>
                </div>
                <Search className="search-input" placeholder="Search..." onSearch={value => this.searchBy(value)} enterButton />
                <Table columns={columns} dataSource={this.props.activeSession!.attendees} />
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        this.props.requestGroups(this.props.groupSearch);
    }

    private hasGroups(): boolean {
        return this.props.paginatedGroupList != null && this.props.paginatedGroupList.list.length > 0;
    }

    private renderEmpty() {
        return <Empty>
        </Empty>;
    }
}

export default connect(
    (state: ApplicationState) => state.groups, // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(Session as any);
