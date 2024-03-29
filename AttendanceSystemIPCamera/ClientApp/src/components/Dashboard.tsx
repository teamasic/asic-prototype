﻿import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { ApplicationState } from '../store';
import { Link, withRouter } from 'react-router-dom';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Breadcrumb, Icon, Button, Empty, Select, List, Card, Spin, Row, Col, Pagination, InputNumber } from 'antd';
import { Typography } from 'antd';
import { Input, Modal, Upload, Table, Divider, message, Form } from 'antd';
import classNames from 'classnames';
import '../styles/Dashboard.css';
import GroupCard from './GroupCard';
import { roomActionCreators, requestRooms } from '../store/room/actionCreators';
import { RoomsState } from '../store/room/state';
import { UnitsState } from '../store/unit/state';
import { sessionActionCreators } from '../store/session/actionCreators';
import { unitActionCreators } from '../store/unit/actionCreators';
import { log, isNullOrUndefined } from 'util';
import { renderStripedTable, success, error } from '../utils'
import { parse } from 'papaparse';
import { FormComponentProps } from 'antd/lib/form';
import TopBar from './TopBar';

const { Search } = Input;
const { Title } = Typography;
const { Text } = Typography;

interface Props extends FormComponentProps {
}

// At runtime, Redux will merge together...
type GroupProps =
    GroupsState
    & Props
    & UnitsState
    & RoomsState// ... state we've requested from the Redux store
    & typeof groupActionCreators
    & typeof roomActionCreators
    & typeof unitActionCreators
    & typeof sessionActionCreators// ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface DashboardComponentState {
    modalVisible: boolean,
    isSaving: boolean,
    importAttendees: any,
    page: number,
    msgImportCSV: string,
    csvFile: File
}

class Dashboard extends React.PureComponent<GroupProps, DashboardComponentState> {

    state = {
        modalVisible: false,
        isSaving: false,
        importAttendees: [],
        page: 1,
        msgImportCSV: ' ',
        csvFile: new File([], 'Null')
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

    public handleOk = (e: any) => {
        e.preventDefault();
        var isImportedFile = this.state.csvFile.name !== 'Null';
        if(!isImportedFile) {
            this.setState({msgImportCSV: 'Please import csv file'});
        }
        this.props.form.validateFields((err: any, values: any) => {
            if (!err && isImportedFile) {
                this.setState({ isSaving: true });
                var newGroup = new Group();
                newGroup.name = values.name;
                newGroup.code = values.code;
                newGroup.attendees = this.state.importAttendees;
                newGroup.totalSession = values.totalSession;
                this.props.postGroup(newGroup, this.reloadGroupList, this.resetModal);
            }
        });
    }

    public reloadGroupList = () => {
        this.props.requestGroups(this.props.groupSearch);
    }

    public resetModal = () => {
        this.setState({
            importAttendees: [],
            page: 1,
            msgImportCSV: ' ',
            csvFile: new File([], 'Null'),
            isSaving: false,
            modalVisible: false
        });
        this.props.form.setFieldsValue({
            code: '',
            name: '',
            totalSession: 30
        });
    }

    public parseFileToTable = (file: File): Promise<void> => {
        return new Promise((resolve, reject) => {
            var thisState = this;
            parse(file, {
                header: true,
                complete: function (results: any, file: File) {
                    if (thisState.checkValidFileFormat(results.data)) {
                        thisState.setState({
                            importAttendees: results.data,
                            msgImportCSV: '',
                            csvFile: file
                        }, () => {
                            resolve();
                        });
                    } else {
                        thisState.setState({
                            importAttendees: [],
                            msgImportCSV: 'You upload a csv file with wrong format. Please try again!'
                        }, () => { reject(); });
                    }
                }, error: function (errors: any, file: File) {
                    thisState.setState({
                        importAttendees: [], 
                        msgImportCSV: "Upload error: " + errors
                    }, () => { reject(); });
                }
            });
        });
    }

    public checkValidFileFormat = (attendees: []) => {
        let temp: { code: string, name: string }[] = attendees;
        if (temp.length > 0) {
            if (!isNullOrUndefined(temp[0].code)
                && !isNullOrUndefined(temp[0].name)) {
                return true;
            }
        }
        return false;
    }

    public handleCancel = () => {
        this.setState({ modalVisible: false });
    }

    public validateBeforeUpload = (file: File): boolean | Promise<void> => {
        if (file.type !== "application/vnd.ms-excel") {
            this.setState({ msgImportCSV: 'Only accept CSV file!' });
            return false;
        }
        return this.parseFileToTable(file);
    }

    public viewDetail = (e: any) => {
        var id = e.target.id;
        this.redirectToGroupDetail(id);
    }

    public redirectToGroupDetail = (id: number) => {
        this.props.history.push(`/group/${id}`);
    }

    public onPageChange = (page: number) => {
        this.setState({
            page: page
        })
    }

    public render() {
        var hasGroups = this.hasGroups();
        const columns = [
            {
                title: "#",
                key: "index",
                width: '5%',
                render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
            },
            {
                title: 'Code',
                key: 'code',
                dataIndex: 'code'
            },
            {
                title: 'Name',
                key: 'name',
                dataIndex: 'name'
            }
        ];
        const { getFieldDecorator } = this.props.form;

        return (
            <React.Fragment>
                <TopBar showHome={false}>
                    <span>
                        Dashboard
                    </span>
                </TopBar>
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
                        centered
                        width='80%'
                        footer={[
                            <Button key="back" onClick={this.handleCancel}>
                                Cancel
                            </Button>,
                            <Button key="submit" type="primary" loading={this.state.isSaving} onClick={this.handleOk}>
                                Save
                            </Button>,
                        ]}
                    >
                        <Form layout="inline">
                            <Row>
                                <Col span={8}>
                                    <Form.Item label="Group Code" required>
                                        {getFieldDecorator('code', {
                                            rules: [
                                                { required: true, message: 'Please input group code' },
                                                { min: 1, max: 50, message: 'Group code requires 1-50 characters' }
                                            ],
                                        })(
                                            <Input placeholder="Enter group code" />
                                        )}
                                    </Form.Item>
                                </Col>
                                <Col span={9}>
                                    <Form.Item label="Group Name" required>
                                        {getFieldDecorator('name', {
                                            rules: [
                                                { required: true, message: 'Please input group name' },
                                                { min: 3, max: 50, message: 'Group name requires 3-50 characters' }
                                            ],
                                        })(
                                            <Input placeholder="Enter group name" />
                                        )}
                                    </Form.Item>
                                </Col>
                                <Col span={7}>
                                    <Form.Item label="Total sessions" required>
                                        {getFieldDecorator('totalSession', {
                                            initialValue: 30,
                                            rules: [{ required: true, message: 'Please input total session' }],
                                        })(<InputNumber
                                            min={0}
                                            max={100}
                                            />
                                        )}
                                    </Form.Item>
                                </Col>
                            </Row>
                        </Form>
                        <Divider orientation="left">List Attendees</Divider>
                        <Row gutter={[0, 32]}>
                            <Col span={4}>
                                <Upload
                                    multiple={false}
                                    accept=".csv"
                                    showUploadList={false}
                                    beforeUpload={this.validateBeforeUpload}
                                >
                                    <Button>
                                        <Icon type="upload" /> Upload CSV File
                                    </Button>
                                </Upload>
                            </Col>
                            <Col span={20}>
                                {this.state.msgImportCSV.length === 0 ?
                                    (
                                        <div>
                                            <Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />
                                            <Text type="secondary"> {this.state.csvFile.name}</Text>
                                        </div>
                                    ) :
                                    this.state.msgImportCSV.length !== 1 ?
                                        (<Icon type="close-circle" theme="twoTone" twoToneColor="#ff0000" />) : null
                                }
                                <Text type="danger"> {this.state.msgImportCSV}</Text>
                            </Col>
                        </Row>
                        <Row>
                            <Table dataSource={this.state.importAttendees}
                                columns={columns} 
                                rowKey="code"
                                bordered
                                rowClassName={renderStripedTable}
                                pagination={{
                                    pageSize: 5,
                                    total: this.state.importAttendees != undefined ? this.state.importAttendees.length : 0,
                                    showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} attendees`,
                                    onChange: this.onPageChange
                                }}
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
        // this.props.requestActiveSession();
        this.props.requestUnits();
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
            <Button type="primary" onClick={this.showModal}>Create a group</Button>
        </Empty>;
    }

    private renderGroupsTable() {
        return (
            <div className="group-grid">
                <div className="fixed-grid--around">
                    {
                        this.props.paginatedGroupList!.list.map(group =>
                            <div key={group.code} className="grid-element">
                                <GroupCard redirect={url => this.redirect(url)}
                                group={group}
                                roomList={this.props.roomList}
                                units={this.props.units}
                                viewDetail={this.viewDetail} />
                            </div>
                        )
                    }
                </div>
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
const mapStateToProps = (state: ApplicationState) => ({ ...state.groups, ...state.rooms, ...state.units })
const mapDispatchToProps = {
    ...roomActionCreators, ...groupActionCreators, ...sessionActionCreators,
    ...unitActionCreators
}

export default Form.create<Props>({ name: 'add_attendee' })
    (withRouter(connect(
        mapStateToProps, // Selects which state properties are merged into the component's props
        mapDispatchToProps // Selects which action creators are merged into the component's props
    )(Dashboard as any)));
