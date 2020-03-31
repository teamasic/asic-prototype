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
import SessionTableView from './SessionTableView';
const { Search } = Input;
const { Title } = Typography;

// At runtime, Redux will merge together...
type SessionProps = SessionState & // ... state we've requested from the Redux store
	typeof sessionActionCreators // ... plus action creators we've requested
	& RouteComponentProps<{
		id?: string;
	}>; // ... plus incoming routing parameters

interface SessionLocalState {
	sessionId: number
}

class Session extends React.PureComponent<SessionProps, SessionLocalState> {
	public constructor(props: SessionProps) {
		super(props);
		this.state = {
			sessionId: 0
		};
	}
	// This method is called when the component is first added to the document
	public componentDidMount() {
		const sessionIdStr = this.props.match.params.id;
		if (sessionIdStr) {
			try {
				const id = parseInt(sessionIdStr);
				this.setState({
					sessionId: id
				});
				this.props.requestSession(id);
			} catch (e) { }
		}
	}

	public markAsPresent(attendeeId: number) {
		const sessionId = this.state.sessionId;
		this.props.createOrUpdateRecord({
			sessionId,
			attendeeId,
			present: true
		});
	}

	public markAsAbsent(attendeeId: number) {
		const sessionId = this.state.sessionId;
		this.props.createOrUpdateRecord({
			sessionId,
			attendeeId,
			present: false
		});
	}

	public render() {
		return (
			<React.Fragment>
				<TopBar>
					{
						this.props.activeSession &&
						<Breadcrumb.Item>
							<Link to={`/group/${this.props.activeSession.groupId}`}>
								<Icon type="hdd" />
								<span>Group</span>
							</Link>
						</Breadcrumb.Item>
					}
					<Breadcrumb.Item>
						<Icon type="calendar" />
						<span>Session</span>
					</Breadcrumb.Item>
				</TopBar>
				<div className={classNames('session-container', {
					'is-loading': this.props.isLoadingSession
				})}>
					{this.props.isLoadingSession ? (
						<Spin size="large" />
					) : this.props.activeSession ? (
						this.renderSessionSection()
					) : (
							this.renderEmpty()
						)}
				</div>
			</React.Fragment>
		);
	}

	private renderSessionSection() {
		const isThisSessionOngoing = this.props.currentlyOngoingSession &&
			this.props.currentlyOngoingSession.id === this.props.activeSession!.id;
		const sessionView = isThisSessionOngoing ?
			this.renderSessionActiveView() :
			this.renderSessionTableView();
		return (
			<React.Fragment>
				<div className="title-container">
					<Title className="title" level={3}>
						Session
					</Title>
					<span className="subtitle">
						{formatFullDateTimeString(this.props.activeSession!.startTime)}
					</span>
				</div>
				{sessionView}
			</React.Fragment>
		);
	}

	private renderSessionTableView() {
		return <SessionTableView
			sessionId={this.state.sessionId}
			markAsAbsent={attendeeId => this.markAsAbsent(attendeeId)}
			markAsPresent={attendeeId => this.markAsPresent(attendeeId)}
		/>
	}

	private renderSessionActiveView() {
		return <></>;
	}

	private renderEmpty() {
		return <Empty></Empty>;
	}
}

export default withRouter(connect(
	(state: ApplicationState, ownProps: SessionProps) => ({
		...state.sessions,
		...ownProps
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(Session as any));
