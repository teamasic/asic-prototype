import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Breadcrumb, Icon, Button, Empty, Select, List, Card, Spin, Row, Col, Pagination } from 'antd';
import { Typography } from 'antd';
import { Input } from 'antd';
import classNames from 'classnames';
import '../styles/Dashboard.css';
import GroupCard from './GroupCard';
import { roomActionCreators, requestRooms } from '../store/room/actionCreators';
import { RoomsState } from '../store/room/state';
import { sessionActionCreators } from '../store/session/actionCreators';

const { Search } = Input;
const { Title } = Typography;

// At runtime, Redux will merge together...
type GroupProps =
    GroupsState
    & RoomsState// ... state we've requested from the Redux store
    & typeof groupActionCreators
    & typeof roomActionCreators
    & typeof sessionActionCreators// ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters


class Dashboard extends React.PureComponent<GroupProps> {
    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public orderBy(value: any) {
        var updatedGroupSearch = {
            ...this.props.groupSearch,
            orderBy: value
        };
        this.props.requestGroups(updatedGroupSearch);
    }

    public searchBy(value: string) {
        var updatedGroupSearch = {
            ...this.props.groupSearch,
            nameContains: value
        };
        this.props.requestGroups(updatedGroupSearch);
    }

    public pageChange(page?: number, pageSize?: number) {
        if (page != null && pageSize != null) {
            var updatedGroupSearch = {
                ...this.props.groupSearch,
                page,
                pageSize
            };
            this.props.requestGroups(updatedGroupSearch);
        }
    }

    public render() {
        console.log(this.props);
        var hasGroups = this.hasGroups();
        return (
            <React.Fragment>
                <div className="breadcrumb-container">
                    <Breadcrumb>
                        <Breadcrumb.Item href="">
                            <Icon type="home" />
                        </Breadcrumb.Item>
                        <Breadcrumb.Item>
                            <Icon type="hdd" />
                            <span>Your groups</span>
                        </Breadcrumb.Item>
                    </Breadcrumb>
                </div>
                <div className="title-container">
                    <Title className="title" level={3}>Your groups</Title>
                    <Button className="new-button" type="primary" icon="plus">
                        New group
                    </Button>
                </div>
                <Row className="filter-container">
                    <Col span={8}>
                        <Search className="search-input" placeholder="Search..." onSearch={value => this.searchBy(value)} enterButton />
                    </Col>
                    <Col span={8} offset={8}>
                        <div>
                            <span className="order-by-sub">Order by:</span>
                            <Select className="order-by-select" defaultValue="DateCreated" onChange={(value: any) => this.orderBy(value)}>
                                <Select.Option value="Name">Name</Select.Option>
                                <Select.Option value="DateCreated">Date created</Select.Option>
                                <Select.Option value="LastSession">Most recent session</Select.Option>
                            </Select>
                        </div>
                    </Col>
                </Row>
                <div className={classNames('group-container', {
                    'empty': !hasGroups,
                    'loading': this.props.isLoading
                })}>
                    {
                        this.props.isLoading ? <Spin size="large" /> :
                        (hasGroups ? this.renderGroupsTable() : this.renderEmpty())
                    }
                </div>
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        this.props.requestGroups(this.props.groupSearch);
        this.props.requestRooms();
        this.props.requestActiveSession();
    }

    private hasGroups(): boolean {
        return this.props.paginatedGroupList != null && this.props.paginatedGroupList.list.length > 0;
    }

    private renderEmpty() {
        return <Empty
            description={
                <span>This is where your groups will show up.</span>
            }
        >
            <Button type="primary">Create a group</Button>
        </Empty>;
    }

    private renderGroupsTable() {
        return (
            <div>
                <List
                    grid={{
                        gutter: 32,
                        xs: 1,
                        sm: 2,
                        md: 3
                    }}
                    dataSource={this.props.paginatedGroupList!.list}
                    renderItem={group => (
                        <List.Item>
                            <GroupCard redirect={url => this.redirect(url)} group={group} roomList={this.props.roomList} />
                        </List.Item>
                    )}
                />
                <div className="pagination-container">
                    <Pagination onChange={(page, pageSize) => this.pageChange(page, pageSize)}
                        defaultCurrent={1} total={this.props.paginatedGroupList!.total} hideOnSinglePage={true} />
                </div>
            </div>
        );
    }

    private redirect(url: string) {
        this.props.history.push(url);
    }
}
const mapStateToProps = (state: ApplicationState) => ({ ...state.groups, ...state.rooms })
const mapDispatchToProps = {
 ...roomActionCreators, ...groupActionCreators, ...sessionActionCreators
}
export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    mapDispatchToProps // Selects which action creators are merged into the component's props
)(Dashboard as any);
