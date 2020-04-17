import * as React from 'react';
import { Button, Popconfirm, Table, Icon } from 'antd';
import { renderStripedTable } from '../utils';
import ScheduleImportModal from './ScheduleImportModal';
import { RouteComponentProps, withRouter } from 'react-router';
import { ApplicationState } from '../store';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { GroupsState } from '../store/group/state';
import moment from 'moment';
import { sessionActionCreators } from '../store/session/actionCreators';
import { SessionState } from '../store/session/state';
import TableConstants from '../constants/TableConstants';

interface Props {
}

interface ScheduleComponentState {
    currentPage: number,
    pageSize: number,
    modalVisible: boolean,
    schedules: any,
    scheduleLoading: boolean
}

type ScheduleProps =
    SessionState
    & GroupsState
    & Props
    & typeof sessionActionCreators
    & RouteComponentProps<{}>;

class Schedule extends React.PureComponent<ScheduleProps, ScheduleComponentState> {
    constructor(props: ScheduleProps) {
        super(props);
        this.state = {
            currentPage: 1,
            pageSize: TableConstants.defaultPageSize,
            modalVisible: false,
            schedules: [],
            scheduleLoading: true
        }
    }

    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.loadSchedules();
    }

    private openModal = () => {
        this.setState({ modalVisible: true });
    }

    private onPageChange = (page: number) => {
        this.setState({ currentPage: page });
    }

    private onShowSizeChange = (current: number, pageSize: number) => {
        this.setState({pageSize: pageSize});
    }

    private handleDelete = (scheduleId: number) => {
        
    }

    private closeModel = () => {
        this.setState({modalVisible: false});
    }

    public render() {
        const columns = [
            {
                title: "#",
                key: "index",
                width: '5%',
                render: (text: any, record: any, index: number) => {
                    return (this.state.currentPage - 1) * this.state.pageSize + index + 1
                }
            },
            {
                title: 'Slot',
                key: 'name',
                dataIndex: 'name'
            },
            {
                title: 'Room',
                key: 'room.name',
                dataIndex: 'room.name'
            },
            {
                title: 'Date',
                key: 'date',
                dataIndex: 'date',
                render: (text: any, record: any, index: number) => {
                    return moment(record.startTime).format('DD-MM-YYYY');
                }
            },
            {
                title: '',
                dataIndex: 'action',
                width: '5%',
                render: (text: any, record: any) =>
                    this.state.schedules != undefined && this.state.schedules.length >= 1 ? (
                        <Popconfirm title="Are you sure to delete this schedule?"
                            onConfirm={() => this.handleDelete(record.id)}>
                            <Button size="small" type="danger" icon="delete"></Button>
                        </Popconfirm>
                    ) : null
                ,
            }
        ];
        return (
            <React.Fragment>
                <Button type="primary"
                    onClick={this.openModal}
                    style={{ marginBottom: 10 }}>
                        <Icon type="schedule" theme="filled" /> Import schedule
                    </Button>
                <ScheduleImportModal
                    modalVisible={this.state.modalVisible}
                    handleCancel={this.closeModel}
                    reloadSchedules={this.loadSchedules}
                />
                <Table dataSource={this.state.schedules}
                    columns={columns}
                    rowKey="id"
                    bordered
                    loading={this.state.scheduleLoading}
                    pagination={{
                        pageSize: 5,
                        total: this.state.schedules != undefined ? this.state.schedules.length : 0,
                        showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} rows`,
                        onChange: this.onPageChange,
                        showSizeChanger: true,
                        onShowSizeChange: this.onShowSizeChange
                    }}
                    rowClassName={renderStripedTable}
                />
            </React.Fragment>
        );
    }

    private loadSchedules = () => {
        this.props.requestGetScheduledSessionByGroupCode(this.props.selectedGroup.code, (data: any) => {
            console.log(data);
            
            this.setState({
                schedules: data,
                scheduleLoading: false
            });
        });
    }
}

const mapStateToProps = (state: ApplicationState, ownProps: Props) => (
    { 
        ...state.sessions, 
        ...state.groups,
        ...ownProps
    })

export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(Schedule as any)