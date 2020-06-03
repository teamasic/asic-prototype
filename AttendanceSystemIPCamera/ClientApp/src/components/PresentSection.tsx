import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { Link, withRouter } from 'react-router-dom';
import { sessionActionCreators } from '../store/session/actionCreators';
import {
    Breadcrumb,
    Icon,
    Button,
    Empty,
    Table,
    Spin,
    Col,
    Tooltip,
    Row,
    Select,
    Radio,
    Modal,
    Input,
    Badge,
    TimePicker,
    Alert
} from 'antd';
import { Typography } from 'antd';
import classNames from 'classnames';
import '../styles/Session.css';
import { SessionState } from '../store/session/state';
import AttendeeRecordPair from '../models/AttendeeRecordPair';
import Record from '../models/Record';
import { formatFullDateTimeString, minutesOfDay, error } from '../utils';
import { takeAttendance } from '../services/session';
import moment from 'moment';
import '../styles/ActiveView.css';
import TopBar from './TopBar';
import MarkUnknownPresentModal from './MarkUnknownPresentModal';
const { Search } = Input;
const { Title, Text } = Typography;
const { confirm } = Modal;

interface Props {
    sessionId: number;
    markAsAbsent: (attendeeCode: string, assumeSuccess: boolean) => void;
}

type PresentProps =
    SessionState &
    typeof sessionActionCreators &
    Props &
    RouteComponentProps<{
        id?: string;
    }>; // ... plus incoming routing parameters

class PresentSection extends React.PureComponent<PresentProps> {

    constructor(props: any) {
        super(props);
    }

    private confirmMarkAbsent(ar: AttendeeRecordPair) {
        confirm({
            title: "Do you want to mark this attendee as absent?",
            okType: "danger",
            onOk: () => {
                this.props.markAsAbsent(ar.attendee.code, false);
            },
        });
    }

    private getImageBox(image: string) {
        return <div className="image-wrapper">
            <div className="placeholder"
                style={{
                    backgroundImage:
                        `url(/api/avatars/placeholder.jpg)`
                }}>
                <div className="image"
                    style={{
                        backgroundImage:
                            image
                    }}>
                </div>
            </div>
        </div>;
    }

    private getRecordImage(ar: AttendeeRecordPair) {
        if (ar.record && ar.record.image) {
            return `url(/api/people/${this.props.sessionId}/${ar.record.image})`;
        } else {
            return `url(/api/avatars/${ar.attendee.image})`;
        }
    }

    public render() {
        const records = this.props.attendeeRecords.filter(ar => ar.record != null);
        const presentRecords = records
            .filter(ar => ar.record!.present)
            .sort((a, b) => b.record!.id - a.record!.id);
        return <div className="container">
            <Title className="title" level={4}>Present attendees</Title>
            {
                presentRecords.length > 0 ?
                    <div className="box-container fixed-grid--around">
                        {
                            presentRecords.map(ar =>
                                <div key={ar.attendee.code}
                                    className="attendee-box grid-element">
                                    <div className="inner-box">
                                        {
                                            this.getImageBox(this.getRecordImage(ar))
                                        }
                                        <div className="inner-box-actions">
                                            <Tooltip title="Mark attendee absent">
                                                <Button
                                                    onClick={() => this.confirmMarkAbsent(ar)}
                                                    size="small"
                                                    type="danger"
                                                    shape="circle"
                                                    icon="close" />
                                            </Tooltip>
                                        </div>
                                    </div>
                                    <div className="code">{ar.attendee.code}</div>
                                    <div className="name">{ar.attendee.name}</div>
                                </div>
                            )
                        }
                    </div> :
                    <Empty description="Attendees will show up here when they are present." />
            }
        </div>;
    }

}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.sessions,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(PresentSection as any);