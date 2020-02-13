import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { sessionActionCreators } from '../store/session/actionCreators';
import { Breadcrumb, Icon, Button, Empty, Table } from 'antd';
import { Typography } from 'antd';
import { Input } from 'antd';
import classNames from 'classnames';
import '../styles/Session.css';
import { SessionState } from '../store/session/state';
import AttendeeRecordPair from '../models/AttendeeRecordPair';

const { Search } = Input;
const { Title } = Typography;

// At runtime, Redux will merge together...
type SessionProps = SessionState & // ... state we've requested from the Redux store
	typeof sessionActionCreators & // ... plus action creators we've requested
	RouteComponentProps<{
		id?: string;
	}>; // ... plus incoming routing parameters

interface SessionLocalState {
	sessionId: number;
}

class Session extends React.PureComponent<SessionProps, SessionLocalState> {
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
			} catch (e) {}
		}
	}

	public searchBy(query: string) {}

	public render() {
		const columns = [
			{
				title: 'Id',
				key: 'id',
				render: (text: string, pair: AttendeeRecordPair) => pair.attendee.code
			},
			{
				title: 'Name',
				key: 'name',
				render: (text: string, pair: AttendeeRecordPair) => pair.attendee.name
			},
			{
				title: 'Present',
				key: 'present',
				render: (text: string, pair: AttendeeRecordPair) => (
					<div>
						<Button type="primary">Present</Button>
					</div>
				)
			},
			{
				title: 'Absent',
				key: 'absent',
				render: (text: string, pair: AttendeeRecordPair) => (
					<div>
						<Button type="danger">Absent</Button>
					</div>
				)
			}
		];
		if (!this.props.activeSession) {
			return <Empty />;
		}
		return (
			<React.Fragment>
				<div className="breadcrumb-container">
					<Breadcrumb>
						<Breadcrumb.Item href="">
							<Icon type="home" />
						</Breadcrumb.Item>
						<Breadcrumb.Item>
							<Icon type="hdd" />
							<span>Group</span>
						</Breadcrumb.Item>
						<Breadcrumb.Item>
							<Icon type="calendar" />
							<span>Session</span>
						</Breadcrumb.Item>
					</Breadcrumb>
				</div>
				<div className="title-container">
					<Title className="title" level={3}>
						Session
					</Title>
				</div>
				<Search
					className="search-input"
					placeholder="Search..."
					onSearch={value => this.searchBy(value)}
					enterButton
				/>
				<Table columns={columns} dataSource={this.props.attendeeRecords} pagination={false} />
			</React.Fragment>
		);
	}

	private renderEmpty() {
		return <Empty></Empty>;
	}
}

export default connect(
	(state: ApplicationState, ownProps: SessionProps) => ({
		...state.sessions,
		...ownProps
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(Session as any);
