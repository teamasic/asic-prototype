import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import { Link, withRouter } from 'react-router-dom';
import { sessionActionCreators } from '../store/session/actionCreators';
import {
	Breadcrumb,
	Icon,
	Empty,
	Spin,
	Col,
	Row,
	Tooltip,
	AutoComplete
} from 'antd';
import { Typography } from 'antd';
import classNames from 'classnames';
import '../styles/Session.css';
import { SessionState } from '../store/session/state';
import { formatFullDateTimeString, error } from '../utils';
import '../styles/Table.css';
import TopBar from './TopBar';
import SessionTableView from './SessionTableView';
import SessionActiveView from './SessionActiveView';
import { EditTwoTone } from '@ant-design/icons';
import { RoomsState } from '../store/room/state';
import Room from '../models/Room';
import { roomActionCreators } from '../store/room/actionCreators';
import UnknownSection from './UnknownSection';
const { Title } = Typography;
const { Option } = AutoComplete

// At runtime, Redux will merge together...
type SessionProps = SessionState &
	RoomsState & // ... state we've requested from the Redux store
	typeof roomActionCreators &
	typeof sessionActionCreators // ... plus action creators we've requested
	& RouteComponentProps<{
		id?: string;
	}>; // ... plus incoming routing parameters

interface SessionLocalState {
	sessionId: number,
	isUpdateRoom: boolean,
	rooms: Room[]
}

class Session extends React.PureComponent<SessionProps, SessionLocalState> {
	public constructor(props: SessionProps) {
		super(props);
		this.state = {
			sessionId: 0,
			isUpdateRoom: false,
			rooms: []
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
		this.loadRooms();
		this.setState({ rooms: this.props.roomList });
	}

	public loadRooms = () => {
		this.props.requestRooms();
	}

	public markAsPresent = (attendeeCode: string, assumeSuccess: boolean = true) => {
		const sessionId = this.state.sessionId;
		this.props.createOrUpdateRecord({
			sessionId,
			attendeeCode,
			present: true
		}, assumeSuccess);
	}

	public markAsAbsent = (attendeeCode: string, assumeSuccess: boolean = true) => {
		const sessionId = this.state.sessionId;
		this.props.createOrUpdateRecord({
			sessionId,
			attendeeCode,
			present: false
		}, assumeSuccess);
	}

	public onUpdateRoom = () => {
		this.setState({ isUpdateRoom: true });
	}

	public onSearchRoom = (value: string) => {
		var results = [];
		if(value != null && value.length > 0) {
			results = this.props.roomList.filter(function(room) {
				return room.name.indexOf(value) === 0;
			});
		} else {
			results = this.props.roomList;
		}
		this.setState({ rooms: results});
	}

	public changeRoom = (value: any) => {
		var existedInList;
		console.log(value);
		console.log(this.props.roomList);
		this.props.roomList.forEach(room => {
			if(room.id == value) {
				existedInList = room;
			}
		});
		console.log(existedInList);
		if(existedInList) {
			var updateRoom = {
				sessionId: this.state.sessionId,
				roomId: value
			};
			this.props.requestUpdateRoom(updateRoom);
			this.setState({ isUpdateRoom: false });
		} else {
			error("Room is not existed!");
		}
	}

	public render() {
		return (
			<React.Fragment>
				<TopBar>
					{
						this.props.activeSession &&
						<Breadcrumb.Item>
							<Link to={`/group/${this.props.activeSession.groupCode}`}>
								<Icon type="team" />
								<span> Group</span>
							</Link>
						</Breadcrumb.Item>
					}
					<Breadcrumb.Item>
						<Icon type="calendar" />
						<span> Session</span>
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

		/*
		const sessionView = this.renderSessionActiveView();
		*/
		const rooms = this.state.rooms.map(room => <Option key={room.id}>{room.name}</Option>);
		return (
			<React.Fragment>
				{/* <div className="title-session-container"> */}
				<Row className="title-session-container">
					<Col span={3}>
						<Title className="title" level={3}>
							Session
							</Title>
					</Col>
					<Col span={4} className="subtitle">
						{formatFullDateTimeString(this.props.activeSession!.startTime)}
					</Col>
					<Col span={2} className="subtitle">
						{this.props.activeSession!.name}
					</Col>
					<Col span={7} className="subtitle">
						{this.state.isUpdateRoom ?
							(
								<AutoComplete
									style={{width: '100px'}}
									defaultValue={this.props.activeSession!.room.name}
									onBlur={this.changeRoom}
									onSearch={this.onSearchRoom}>
									{rooms}
								</AutoComplete>
							) :
							(
								<div>
									<span>Room {this.props.activeSession!.room.name}</span>
									<Tooltip title="Change room">
										<EditTwoTone onClick={this.onUpdateRoom} />
									</Tooltip>
								</div>
							)
						}
					</Col>
				</Row>
				{/* </div> */}
				{sessionView}
			</React.Fragment>
		);
	}

	private renderSessionTableView() {
		return <div>
			<SessionTableView
				sessionId={this.state.sessionId}
				markAsAbsent={this.markAsAbsent}
				markAsPresent={this.markAsPresent}
			/>
			<UnknownSection sessionId={this.state.sessionId}
				markAsPresent={this.markAsPresent} />
		</div>
	}

	private renderSessionActiveView() {
		return <SessionActiveView
			sessionId={this.state.sessionId}
			markAsAbsent={this.markAsAbsent}
			markAsPresent={this.markAsPresent}
		/>;
	}

	private renderEmpty() {
		return <Empty></Empty>;
	}
}

const mapDispatchToProps = {
	...roomActionCreators, ...sessionActionCreators
}

export default withRouter(connect(
	(state: ApplicationState, ownProps: SessionProps) => ({
		...state.sessions,
		...state.rooms,
		...ownProps
	}), // Selects which state properties are merged into the component's props
	mapDispatchToProps // Selects which action creators are merged into the component's props
)(Session as any));
