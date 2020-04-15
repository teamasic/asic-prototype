import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Unit from '../models/Unit';
import { ApplicationState } from '../store';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { ChangeRequestState } from '../store/changeRequest/state';
import * as moment from 'moment'
import Room from '../models/Room';
import { createSession } from '../services/session';
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Select, InputNumber, Typography, Modal, TimePicker, message, Input } from 'antd';
import { formatFullDateTimeString, formatDateString } from '../utils';
import classNames from 'classnames';
import { createBrowserHistory } from 'history';
import ChangeRequest, { ChangeRequestStatus } from '../models/ChangeRequest';
import { Link } from 'react-router-dom';

const { Title } = Typography;
const { Option } = Select;
const { confirm, success, error } = Modal;

interface Props {
    changeRequest: ChangeRequest;
    visible: boolean;
    onClose: () => void;
}

// At runtime, Redux will merge together...
type ModalProps = Props &
    ChangeRequestState & // ... state we've requested from the Redux store
    typeof changeRequestActionCreators & // ... plus action creators we've requested
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class ChangeRequestModal extends React.PureComponent<ModalProps> {
    public state = {
    };

    private showError() {
        error({
            title: 'Error in processing change request',
            content: 'Please try again.'
        });
    }

    private showSuccess() {
        this.props.onClose();
        success({
            title: 'Successfully processed change request'
        });
    }

    private approveChangeRequest(recordId: number) {
        this.props.processChangeRequest(recordId, true, this.showSuccess.bind(this), this.showError);
    }

    private rejectChangeRequest(recordId: number) {
        this.props.processChangeRequest(recordId, false, this.showSuccess.bind(this), this.showError);
    }

    private confirmApprove = () => {
        const { attendeeName, recordId } =  this.props.changeRequest;
        confirm({
            title: "Do you want to approve this change request?",
            content: `${attendeeName} will be marked present for this session. You can always change your mind later.`,
            icon: <Icon type="exclamation-circle" />,
            onOk: () => this.approveChangeRequest(recordId)
        });
    }

    private confirmReject = () => {
        const { attendeeName, recordId } = this.props.changeRequest;
        confirm({
            title: "Do you want to reject this change request?",
            content: `${attendeeName} will be marked absent for this session. You can always change your mind later.`,
            icon: <Icon type="exclamation-circle" />,
            onOk: () => this.rejectChangeRequest(recordId)
        });
    }

    public render() {
        const cr = this.props.changeRequest;
        if (cr == null) return <></>;
        let status;
        switch (cr.status) {
            case ChangeRequestStatus.APPROVED:
                status = <>
                    <Icon type="check-circle" />
                    <span className="status">Approved</span>
                </>;
                break;
            case ChangeRequestStatus.REJECTED:
                status = <>
                    <Icon type="close-circle" />
                    <span className="status">Rejected</span>
                </>;
                break;
            case ChangeRequestStatus.UNRESOLVED:
                status = <>
                    <Icon type="question-circle" />
                    <span className="status">Unresolved</span>
                </>;
                break;
        }
        return (
            <Modal
                title="Change request details"
                className="change-request-modal"
                visible={this.props.visible}
                onOk={() => { }}
                onCancel={() => this.props.onClose()}
                footer={[
                    <Button key="submit"
                        disabled={cr.status === ChangeRequestStatus.APPROVED}
                        type="primary" onClick={() => this.confirmApprove()}>
                        Approve
                    </Button>,
                    <Button key="back"
                        type="danger"
                        disabled={cr.status === ChangeRequestStatus.REJECTED}
                        onClick={() => this.confirmReject()}>
                        Reject
                    </Button>
                ]}
            >
                <Row className="info-wrapper">
                    <Col span={4}><span className="label shifted-down">Group:</span></Col>
                    <Col span={20} className="info">{cr.groupCode + " - " + cr.groupName}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col span={4}><span className="label shifted-down">Attendee:</span></Col>
                    <Col span={20} className="info">{cr.attendeeName}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col span={4}><span className="label shifted-down">Date:</span></Col>
                    <Col span={20} className="info">{formatDateString(cr.sessionTime)}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col span={4}><span className="label">Status:</span></Col>
                    <Col className="">{status}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col span={4}><span className="label">Comment:</span></Col>
                    <Col span={20} className="comment-wrapper">{cr.comment}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col offset={4}>
                        <Link to={`session/${cr.sessionId}`}>
                            <Button className="to-session-button" type="link">
                                Go to session
                                <Icon type="arrow-right" />
                            </Button>
                        </Link>
                    </Col>
                </Row>
            </Modal>
        );
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.groups,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators({
        ...changeRequestActionCreators
    }, dispatch) // Selects which action creators are merged into the component's props
)(ChangeRequestModal as any);
