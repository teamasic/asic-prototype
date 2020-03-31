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
import { formatFullDateTimeString, minutesOfDay, error } from '../utils';
import { takeAttendance } from '../services/session';
import moment from 'moment';
import '../styles/Table.css';
import TopBar from './TopBar';
import TakeAttendanceModal from './TakeAttendanceModal';
const { Search } = Input;
const { Title } = Typography;

interface Props {
	sessionId: number;
	markAsPresent: (attendeeId: number) => void;
	markAsAbsent: (attendeeId: number) => void;
}

// At runtime, Redux will merge together...
type SessionProps = Props & SessionState & // ... state we've requested from the Redux store
	typeof sessionActionCreators // ... plus action creators we've requested
	& RouteComponentProps<{
		id?: string;
	}>; // ... plus incoming routing parameters

interface State {
}

class SessionActiveView extends React.PureComponent<SessionProps, State> {
	public constructor(props: SessionProps) {
		super(props);
		this.state = {
		};
	}

	public render() {
		return (
			<div>
				Session active view 
			</div>
		);
	}
}

export default connect(
	(state: ApplicationState, ownProps: Props) => ({
		...state.sessions,
		...ownProps
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(SessionActiveView as any);
