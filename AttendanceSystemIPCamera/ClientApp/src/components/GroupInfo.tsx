import * as React from 'react';
import { Table, Popconfirm, Button, message, Modal, Form, Input, Typography } from 'antd'
import { GroupsState } from '../store/group/state';
import Group from '../models/Group';
import { groupActionCreators } from '../store/group/actionCreators';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import Attendee from '../models/Attendee';
import '../styles/Table.css';
import { renderStripedTable } from '../utils'

const { Text } = Typography

interface Props {
    attendees?: Attendee[],
    attendeeLoading: boolean
}

interface GroupInfoComponentstate {
    modalVisible: boolean,
    newAttendee: Attendee,
    page: number
}

// At runtime, Redux will merge together...
type GroupInfoProps =
    Props 
    & GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class GroupInfo extends React.PureComponent<GroupInfoProps, GroupInfoComponentstate> {
    state = {
        modalVisible: false,
        newAttendee: {
            id: 0,
            code: '',
            name: ''
        },
        page: 1
    }

    public handleDelete = (attendeeId: number) => {
        this.props.startDeleteAttendeeGroup(attendeeId,
            this.props.selectedGroup.id,
            message.success("Delete attendee success!"));
    }

    public addAttendee = () => {
        this.props.startCreateAttendeeInGroup(this.props.selectedGroup.id,
            this.state.newAttendee,
            this.addAttendeeSuccess,
            this.duplicateAttendee);
    }

    public addAttendeeSuccess = () => {
        var attendee = {
            ...this.state.newAttendee,
            id: 0,
            code: '',
            name: ''
        };
        this.setState({
            modalVisible: false,
            newAttendee: attendee
        });
        message.success("Add attendee success!");
    }

    public openModal = () => {
        this.setState({
            modalVisible: true
        })
    }

    public handleCancel = () => {
        this.setState({
            modalVisible: false
        });
    }

    public onNewCodeChange = (e: any) => {
        var attendee = {
            ...this.state.newAttendee,
            code: e.target.value
        };
        this.setState({
            newAttendee: attendee
        })
    }

    public onNewNameChange = (e: any) => {
        var attendee = {
            ...this.state.newAttendee,
            name: e.target.value
        };
        this.setState({
            newAttendee: attendee
        })
    }

    public getAttendeeName = () => {
        this.props.startGetAttendeeByCode(this.state.newAttendee.code, this.loadAttendeeName);
    }

    public loadAttendeeName = (name: string) => {
        var attendee = {
            ...this.state.newAttendee,
            name: name
        };
        console.log(attendee);
        this.setState({
            newAttendee: attendee
        })
    } 

    public onPageChange = (page: number) => {
        this.setState({
            page: page
        })
    }

    public duplicateAttendee = () => {
        message.error("Attendee " + this.state.newAttendee.code + " is already in this group!");
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
                title: 'Code',
                key: 'code',
                dataIndex: 'code'
            },
            {
                title: 'Name',
                key: 'name',
                dataIndex: 'name'
            },
            {
                title: '',
                dataIndex: 'action',
                width: '5%',
                render: (text: any, record: any) => 
                    this.props.attendees != undefined && this.props.attendees.length >= 1 ? (
                        <Popconfirm title="Are you sure to delete this attendee?"
                            onConfirm={() => this.handleDelete(record.id)}>
                            <Button size="small" type="danger" icon="delete"></Button>
                        </Popconfirm>
                    ): null
                ,
            }
        ];
        
        return (
            <div>
                <Button type="primary"
                    icon="user-add"
                    onClick={this.openModal}
                    style={{ marginBottom: 10 }}>
                    Add an attendee
                    </Button>
                <Modal
                    visible={this.state.modalVisible}
                    title="Add new attendee"
                    okText="Save"
                    onOk={this.addAttendee}
                    onCancel={this.handleCancel}
                >
                    <Form labelCol={{ span: 5 }} wrapperCol={{ span: 12 }} onSubmit={this.addAttendee}>
                        <Form.Item label="Code" required>
                            <Input type="text" onChange={this.onNewCodeChange} value={this.state.newAttendee.code} onBlur={this.getAttendeeName} />
                        </Form.Item>
                        <Form.Item label="Name" required>
                            <Input type="text" onChange={this.onNewNameChange} value={this.state.newAttendee.name} />
                        </Form.Item>
                    </Form>
                </Modal>
                <Table dataSource={this.props.attendees}
                    columns={columns}
                    rowKey="$id"
                    bordered
                    loading={this.props.attendeeLoading}
                    pagination={{
                        pageSize: 5,
                        total: this.props.attendees != undefined ? this.props.attendees.length : 0,
                        showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} attendees`,
                        onChange: this.onPageChange
                    }}
                    rowClassName={renderStripedTable}
                />
            </div>
            
            );
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.groups,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(GroupInfo as any);