import * as React from 'react';
import { Button, Popconfirm, Table, Icon } from 'antd';
import { renderStripedTable } from '../utils';
import ScheduleImportModal from './ScheduleImportModal';

interface Props {
    groupId: number
}

interface ScheduleComponentState {
    page: number,
    modalVisible: boolean,
    schedules: any,
    scheduleLoading: boolean
}

class Schedule extends React.PureComponent<Props, ScheduleComponentState> {
    constructor(props: Props) {
        super(props);
        this.state = {
            page: 1,
            modalVisible: false,
            schedules: [],
            scheduleLoading: false
        }
    }

    private openModal = () => {
        this.setState({ modalVisible: true });
    }

    private onPageChange = (page: number) => {
        this.setState({ page: page });
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
                render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
            },
            {
                title: 'Slot',
                key: 'slot',
                dataIndex: 'slot'
            },
            {
                title: 'Room',
                key: 'room',
                dataIndex: 'room'
            },
            {
                title: 'Date',
                key: 'date',
                dataIndex: 'date'
            },
            {
                title: '',
                dataIndex: 'action',
                width: '5%',
                render: (text: any, record: any) =>
                    this.state.schedules != undefined && this.state.schedules.length >= 1 ? (
                        <Popconfirm title="Are you sure to delete this attendee?"
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
                />
                <Table dataSource={this.state.schedules}
                    columns={columns}
                    rowKey="$id"
                    bordered
                    loading={this.state.scheduleLoading}
                    pagination={{
                        pageSize: 5,
                        total: this.state.schedules != undefined ? this.state.schedules.length : 0,
                        showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} rows`,
                        onChange: this.onPageChange
                    }}
                    rowClassName={renderStripedTable}
                />
            </React.Fragment>
        );
    }
}

export default Schedule;