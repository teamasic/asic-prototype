import * as React from 'react';
import { Table, Popconfirm, Button, message, Modal, Form, Input } from 'antd'
import { GroupsState } from '../store/group/state';
import { groupActionCreators } from '../store/group/actionCreators';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import Attendee from '../models/Attendee';
import '../styles/Table.css';
import { renderStripedTable, success, error } from '../utils'
import { FormComponentProps } from 'antd/lib/form';

interface Props extends FormComponentProps {
    attendees?: Attendee[],
    attendeeLoading: boolean
}

interface GroupInfoComponentstate {
    modalVisible: boolean,
    newAttendee: Attendee,
    currentPage: number,
    pageSize: number
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
            name: '',
            image: ''
        },
        currentPage: 1,
        pageSize: 10
    }

    public handleDelete = (attendeeCode: string) => {
        this.props.startDeleteAttendeeGroup(attendeeCode,
            this.props.selectedGroup.code,
            message.success("Delete attendee success!"));
    }

    public addAttendee = () => {
        this.props.startCreateAttendeeInGroup(this.props.selectedGroup.code,
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
        this.props.form.setFieldsValue({
            attendeeCode: '',
            attendeeName: ''
        });
        success("Add attendee to group success!");
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
        this.setState({
            newAttendee: attendee
        })
        this.props.form.setFieldsValue({
            attendeeName: name
        });
    }

    public onPageChange = (page: number) => {
        this.setState({ currentPage: page });
    }

    public onShowSizeChange = (current: number, pageSize: number) => {
        console.log("current: " + current);
        
        this.setState({pageSize: pageSize});
    }

    public duplicateAttendee = () => {
        error("Attendee " + this.state.newAttendee.code + " is already in this group!");
    }

    public handleSubmit = (e: any) => {
        e.preventDefault();
        this.props.form.validateFields((err: any, values: any) => {
            if (!err) {
                console.log('Received values of form: ', values);
                this.addAttendee();
            }
        });
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
                            onConfirm={() => this.handleDelete(record.code)}>
                            <Button size="small" type="danger" icon="delete"></Button>
                        </Popconfirm>
                    ) : null
                ,
            }
        ];
        const { getFieldDecorator } = this.props.form;
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
                    onOk={this.handleSubmit}
                    onCancel={this.handleCancel}
                >
                    <Form labelCol={{ span: 5 }} wrapperCol={{ span: 12 }} onSubmit={this.handleSubmit}>
                        <Form.Item label="Code" required>
                            {getFieldDecorator('attendeeCode', {
                                rules: [{ required: true, message: 'Please input attendee code' }],
                            })(
                                <Input type="text" onChange={this.onNewCodeChange}
                                    onBlur={this.getAttendeeName} />
                            )}
                        </Form.Item>
                        <Form.Item label="Name" required>
                            {getFieldDecorator('attendeeName', {
                                rules: [{ required: true, message: 'Please input attendee name' }],
                            })(
                                <Input type="text" onChange={this.onNewNameChange} />
                            )}
                        </Form.Item>
                    </Form>
                </Modal>
                <Table dataSource={this.props.attendees}
                    columns={columns}
                    rowKey="code"
                    bordered
                    loading={this.props.attendeeLoading}
                    pagination={{
                        pageSize: this.state.pageSize,
                        total: this.props.attendees != undefined ? this.props.attendees.length : 0,
                        showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} attendees`,
                        onChange: this.onPageChange,
                        showSizeChanger: true,
                        onShowSizeChange: this.onShowSizeChange
                    }}
                    rowClassName={renderStripedTable}
                />
            </div>

        );
    }
}

export default Form.create<Props>({ name: 'add_attendee' })
    (connect(
        (state: ApplicationState, ownProps: Props) => ({
            ...state.groups,
            ...ownProps
        }), // Selects which state properties are merged into the component's props
        dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
    )(GroupInfo as any));