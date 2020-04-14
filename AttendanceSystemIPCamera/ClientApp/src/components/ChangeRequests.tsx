import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { Link, withRouter } from 'react-router-dom';
import Group from '../models/Group';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Breadcrumb, Icon, Button, Empty, Select, List, Card, Spin, Row, Col, Pagination } from 'antd';
import { Typography } from 'antd';
import { Input, Modal, Upload, Table, Divider, message } from 'antd';
import classNames from 'classnames';
import '../styles/ChangeRequests.css';
import { formatDateString, renderStripedTable } from '../utils';
import GroupCard from './GroupCard';
import { roomActionCreators, requestRooms } from '../store/room/actionCreators';
import { ChangeRequestState } from '../store/changeRequest/state';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { log, isNullOrUndefined } from 'util';
import { parse } from 'papaparse';
import ChangeRequest, { ChangeRequestStatusFilter, ChangeRequestStatus } from '../models/ChangeRequest';
import ChangeRequestModal from './ChangeRequestModal';
import '../styles/Table.css';
import TopBar from './TopBar';

const { Search } = Input;
const { Title } = Typography;
const { Text } = Typography;

// At runtime, Redux will merge together...
type ChangeRequestsComponentProps =
    ChangeRequestState // ... state we've requested from the Redux store
    & typeof changeRequestActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface ChangeRequestsComponentState {
    modalVisible: boolean,
    activeChangeRequest?: ChangeRequest,
    filter: ChangeRequestStatusFilter;
}

class ChangeRequests extends React.PureComponent<ChangeRequestsComponentProps, ChangeRequestsComponentState> {

    constructor(props: ChangeRequestsComponentProps) {
        super(props);
        this.state = {
            modalVisible: false,
            filter: ChangeRequestStatusFilter.ALL
        };
    }

    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    private ensureDataFetched() {
        this.props.requestChangeRequests(ChangeRequestStatusFilter.ALL);
    }

    public filterBy = (value: ChangeRequestStatusFilter) => {
        if (value != this.state.filter) {
            this.props.requestChangeRequests(value);
            this.setState({
                filter: value
            });
        }
    }

    public render() {
        const hasChangeRequests = this.props.changeRequests != null && this.props.changeRequests.length > 0;
        return (
            <React.Fragment>
                <TopBar>
                    <Breadcrumb.Item>
                        <Icon type="calendar" />
                        <span>Your change requests</span>
                    </Breadcrumb.Item>
                </TopBar>
                <div className="title-container">
                    <Title className="title" level={3}>Your change requests</Title>                    
                </div>
                <Row className="filter-container">
                    <Col span={8} offset={16}>
                        <div>
                            <span className="order-by-sub">Filter by:</span>
                            <Select className="order-by-select" defaultValue={ChangeRequestStatusFilter.ALL} onChange={(value: ChangeRequestStatusFilter) => this.filterBy(value)}>
                                <Select.Option value={ChangeRequestStatusFilter.ALL}>All</Select.Option>
                                <Select.Option value={ChangeRequestStatusFilter.UNRESOLVED}>Unresolved</Select.Option>
                                <Select.Option value={ChangeRequestStatusFilter.RESOLVED}>Resolved</Select.Option>
                            </Select>
                        </div>
                    </Col>
                </Row>
                {
                    this.state.activeChangeRequest != null && 
                    <ChangeRequestModal
                        visible={this.state.modalVisible}
                        changeRequest={this.state.activeChangeRequest}
                        onClose={() => this.closeModal()} />
                }
                <div className={classNames('group-container', {
                    'empty': !hasChangeRequests,
                    'loading': this.props.isLoading
                })}>
                    {
                        this.props.isLoading ? <Spin size="large" /> :
                            (hasChangeRequests ? this.renderTable() : this.renderEmpty())
                    }
                </div>
            </React.Fragment>
        );
    }

    private renderTable() {
        const columns = [
            {
                title: 'No.',
                key: 'no',
                render: (text: string, cr: ChangeRequest, i: number) => (i+1)
            },
            {
                title: 'Group',
                key: 'group',
                render: (text: string, cr: ChangeRequest) => cr.groupCode + " " + cr.groupName
            },
            {
                title: 'Session',
                key: 'session',
                render: (text: string, cr: ChangeRequest) => cr.sessionName
            },
            {
                title: 'Date',
                key: 'date',
                render: (text: string, cr: ChangeRequest) => formatDateString(cr.sessionTime)
            },
            {
                title: 'Attendee code',
                key: 'code',
                render: (text: string, cr: ChangeRequest) => cr.attendeeCode
            },
            {
                title: 'Attendee name',
                key: 'name',
                render: (text: string, cr: ChangeRequest) => cr.attendeeName
            },
            {
                title: 'Status',
                key: 'status',
                render: (text: string, cr: ChangeRequest) => {
                    switch (cr.status) {
                        case ChangeRequestStatus.APPROVED:
                            return <span className="badge approved">Approved</span>;
                        case ChangeRequestStatus.REJECTED:
                            return <span className="badge rejected">Rejected</span>;
                        case ChangeRequestStatus.UNRESOLVED:
                            return <span className="badge unresolved">Unresolved</span>;
                        default:
                            return <div></div>;
                    }
                }
            },
            {
                title: 'Review',
                key: 'review',
                render: (text: string, cr: ChangeRequest) =>
                    <Button className="review-btn" type="link" onClick={() => this.showModal(cr)}>Review</Button>
            }
        ];
        return <Table
            columns={columns}
            dataSource={this.props.changeRequests.sort((a, b) => b.id - a.id)}
            rowKey={cr => cr.id.toString()}
            rowClassName={renderStripedTable}
        />;
    }

    private showModal(cr: ChangeRequest) {
        this.setState({
            modalVisible: true,
            activeChangeRequest: cr
        });
    }

    private closeModal() {
        this.setState({
            modalVisible: false
        });
    }

    private renderEmpty() {
        return <Empty
            description={
                <span>No change request found.</span>
            }
        >
        </Empty>;
    }
}
const mapStateToProps = (state: ApplicationState) => ({ ...state.changeRequests })
const mapDispatchToProps = {
    ...changeRequestActionCreators
}
export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    mapDispatchToProps // Selects which action creators are merged into the component's props
)(ChangeRequests as any);
