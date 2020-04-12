import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import AttendeeRecordPair from '../models/AttendeeRecordPair';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import * as moment from 'moment'
import Room from '../models/Room';
import Attendee from '../models/Attendee';
import { createSession } from '../services/session';
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Select, InputNumber, Typography, Modal, TimePicker, message, Tooltip } from 'antd';
import { formatFullDateTimeString, success, error } from '../utils';
import classNames from 'classnames';
import { createBrowserHistory } from 'history';

const { Text } = Typography;
const { Option } = Select;
const { confirm } = Modal;


interface ModalProps {
    unknownImage: string;
    visible: boolean;
    hideModal: () => void;
    attendeeRecords: AttendeeRecordPair[];
    markAsPresent: (attendeeId: number) => void;
    removeUnknownImage: (image: string) => void;
}

interface State {
    attendeeId: number;
    isError: boolean;
}

class MarkUnknownPresentModal extends React.PureComponent<ModalProps, State> {
    constructor(props: ModalProps) {
        super(props);
        this.state = {
            attendeeId: -1,
            isError: false
        };
    }

    private handleModalOk = async () => {
        const { attendeeId } = { ...this.state };
        if (attendeeId == -1) {
            error("Please select an attendee.");
            return;
        }
        confirm({
            title: "Do you want to mark this attendee as present?",
            okType: "primary",
            onOk: () => {
                const pair = this.props.attendeeRecords
                    .find(ar => ar.attendee.id === attendeeId);
                const present = pair && pair.record != null && pair.record.present;
                if (!present) {
                    this.props.markAsPresent(attendeeId);
                }
                this.props.removeUnknownImage(this.props.unknownImage);
                this.props.hideModal();
            },
        });
    }

    private handleModalCancel = () => {
        this.props.hideModal();
    }

    private onChangeAttendee = (id: number) => {
        this.setState({
            attendeeId: id
        })
    }
    private renderAttendeeOptions = () => {
        return this.props.attendeeRecords.map(ar => {
            const present = ar.record != null && ar.record.present;
            return <Option key={ar.attendee.id}
                className={classNames({
                    'option-present': present
                })}
                value={ar.attendee.id}>
                {ar.attendee.code} - {ar.attendee.name}
            </Option>
        })
    }

    public render() {
        return <Modal
            title="Mark attendee as present"
            visible={this.props.visible}
            onOk={this.handleModalOk}
            onCancel={this.handleModalCancel}>
            <div>
                <Row type="flex" justify="start" align="top" gutter={[16, 16]}>
                    <Col span={6}><p>Choose attendee</p></Col>
                    <Col span={16}>
                        <Select
                            showSearch
                            style={{ width: '100%' }}
                            placeholder="Select attendee"
                            optionFilterProp="children"
                            onChange={(id: any) => this.onChangeAttendee(id)}
                            filterOption={(input: string, option: any) => {
                                const children: string[] = option.props.children;
                                const label = children.join(' ');
                                return label.toLowerCase().includes(input.toLowerCase());
                            }}
                        >
                            {this.renderAttendeeOptions()}
                        </Select>
                        <Text type="secondary">Attendees with blue names are already present.</Text>
                    </Col>
                </Row>
                {this.state.isError ? <Row type="flex" justify="start" align="top" gutter={[16, 16]}>
                    <Col span={14}><p style={{ color: "red" }}>Please choose an attendee</p></Col>
                </Row> : null}
            </div>
        </Modal>;
    }
}

export default MarkUnknownPresentModal;
