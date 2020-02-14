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
import { Input, Modal, Upload, Table, Divider } from 'antd';
import classNames from 'classnames';
import '../styles/Dashboard.css';
import GroupCard from './GroupCard';
import { log } from 'util';
import { parse } from 'papaparse';

const { Search } = Input;
const { Title } = Typography;
const { Text } = Typography;

// At runtime, Redux will merge together...
type GroupProps =
    GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface DashboardComponentState {
    modalVisible: boolean,
    modalLoading: boolean,
    importAttendees: any, 
    groupCode: string,
    groupName: string
}

class Dashboard extends React.PureComponent<GroupProps, DashboardComponentState> {

    state = {
        modalVisible: false,
        modalLoading: false,
        importAttendees: [],
        groupCode: "",
        groupName: ""
    }

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

    public showModal = () => {
        this.setState({
            modalVisible: true
        });
    }

    public handleOk = (value: any) => {
        var newGroup = new Group();
        newGroup.name = this.state.groupName;
        newGroup.code = this.state.groupCode;
        newGroup.attendees = this.state.importAttendees;
        //newGroup.attendees.pop();
        console.log(newGroup);
        this.props.postGroup(newGroup);
    }

    public parseFileToTable = (file: File) => {
        var thisState = this;
        parse(file, {
            header: true,
            complete: function (results: any, file: File) {
                console.log(results.data);
                thisState.setState({
                    importAttendees: results.data
                });
            }
        });
    }

    public handleCancel = () => {
        this.setState({
            modalVisible: false,
            importAttendees: []
        });
    }

    public onGroupCodeChange = (e: any) => {
        this.setState({
            groupCode: e.target.value
        });
    } 

    public onGroupNameChange = (e: any) => {
        this.setState({
            groupName: e.target.value
        });
    }

    public validateBeforeUpload = (file: File) => {
        if (file.type !== "application/vnd.ms-excel") {
            return false;
        }
        return true;
    }

    public render() {
        var hasGroups = this.hasGroups();
        const columns = [
            {
                title: 'No.',
                key: 'No',
                dataIndex: 'No'
            },
            {
                title: 'Code',
                key: 'Code',
                dataIndex: 'Code'
            },
            {
                title: 'Name',
                key: 'Name',
                dataIndex: 'Name'
            }
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
                            <span>Your groups</span>
                        </Breadcrumb.Item>
                    </Breadcrumb>
                </div>
                <div className="title-container">
                    <Title className="title" level={3}>Your groups</Title>
                    <Button className="new-button" type="primary" icon="plus" onClick={this.showModal}>
                        New group
                    </Button>
                    <Modal
                        visible={this.state.modalVisible}
                        title="Create New Group"
                        onOk={this.handleOk}
                        onCancel={this.handleCancel}
                        style={{ top: 20 }}
                        width={900}
                        footer={[
                            <Button key="back" onClick={this.handleCancel}>
                                Cancel
                            </Button>,
                            <Button key="submit" type="primary" loading={this.state.modalLoading} onClick={this.handleOk}>
                                            Save
                            </Button>,
                        ]}
                    >
                        <Row>
                            <Col span={8}><Text strong>Group Code:</Text></Col>
                            <Col span={4} offset={2}><Text strong>Group Name:</Text></Col>
                        </Row>
                        <Row>
                            <Col span={8}>
                                <Input placeholder="Enter group code" onChange={this.onGroupCodeChange} />
                            </Col>
                            <Col span={12} offset={2}>
                                <Input placeholder="Enter group name" onChange={this.onGroupNameChange} />
                            </Col>
                        </Row>
                        <Divider orientation="left">List Attendees</Divider>
                        <Row gutter={[0, 32]}>
                            <Col span={8}>
                                <Upload
                                    multiple={false}
                                    accept=".csv"
                                    action="https://www.mocky.io/v2/5cc8019d300000980a055e76"
                                    showUploadList={false}
                                    beforeUpload={this.validateBeforeUpload}
                                    data={this.parseFileToTable}
                                >
                                    <Button>
                                        <Icon type="upload" /> Upload CSV File
                                    </Button>
                                </Upload>
                            </Col>
                        </Row>
                        <Row>
                            <Table dataSource={this.state.importAttendees} columns={columns} rowKey="No"
                                pagination={{ pageSize: 5 }}
                            />;
                        </Row>
                    </Modal>
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
                    'empty': !hasGroups
                })}>
                    {
                        this.props.isLoading ? <Spin /> :
                            (hasGroups ? this.renderGroupsTable() : this.renderEmpty())
                    }
                </div>
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
                            <GroupCard group={group} />
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
}

export default connect(
    (state: ApplicationState) => state.groups, // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(Dashboard as any);
