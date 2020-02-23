import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
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
	Radio
} from 'antd';
import { Typography } from 'antd';
import { Input } from 'antd';
import classNames from 'classnames';
import '../styles/Session.css';
import { SessionState } from '../store/session/state';
import AttendeeRecordPair from '../models/AttendeeRecordPair';
import { formatFullDateTimeString } from '../utils';

const { Search } = Input;
const { Title } = Typography;

// At runtime, Redux will merge together...
type SessionProps = SessionState & // ... state we've requested from the Redux store
	typeof sessionActionCreators & // ... plus action creators we've requested
	RouteComponentProps<{
		id?: string;
	}>; // ... plus incoming routing parameters

enum FilterBy {
	PRESENT,
	ABSENT,
	NOT_YET,
	ALL
}

interface SessionLocalState {
	sessionId: number;
	search: {
		query: string;
		filter: FilterBy;
	};
}

class Session extends React.PureComponent<SessionProps, SessionLocalState> {
	public constructor(props: SessionProps) {
		super(props);
		this.state = {
			sessionId: 0,
			search: {
				query: '',
				filter: FilterBy.ALL
			}
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
			} catch (e) {}
		}
	}

	public searchBy(query: string) {
		this.setState({
			search: {
				...this.state.search,
				query
			}
		});
	}

	public filterBy(filter: FilterBy) {
		this.setState({
			search: {
				...this.state.search,
				filter
			}
		});
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

	private searchAttendeeList(
		attendeeRecords: AttendeeRecordPair[],
		query: string
	) {
		return attendeeRecords.filter(
			ar => ar.attendee.name.includes(query) || ar.attendee.code.includes(query)
		);
	}

	private filterAttendeeList(
		attendeeRecords: AttendeeRecordPair[],
		filter: FilterBy
	) {
		switch (filter) {
			case FilterBy.ALL:
				return attendeeRecords;
			case FilterBy.NOT_YET:
				return attendeeRecords.filter(ar => ar.record == null);
			case FilterBy.PRESENT:
				return attendeeRecords.filter(
					ar => ar.record != null && ar.record!.present
				);
			case FilterBy.ABSENT:
				return attendeeRecords.filter(
					ar => ar.record != null && !ar.record!.present
				);
		}
	}

	public render() {
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
				title: <div className="actions">
					Present
					<div className="buffer"></div>
					Absent
				</div>,
				key: 'present',
				render: (text: string, pair: AttendeeRecordPair) =>
					<div className="actions">
						<Radio
							checked={pair.record != null && pair.record.present}
							onChange={() => this.markAsPresent(pair.attendee.id)}></Radio>
						<div className="big-buffer"></div>
						<Radio
							checked={pair.record != null && !pair.record.present}
							onChange={() => this.markAsAbsent(pair.attendee.id)}></Radio>
					</div>
			},
			/*
			{
				title: 'Absent',
				key: 'absent',
				render: (text: string, pair: AttendeeRecordPair) =>
					<Radio
						checked={pair.record != null && !pair.record.present}
						onChange={() => this.markAsAbsent(pair.attendee.id)}></Radio>
			}*/
		];
		const processedList = this.searchAttendeeList(
			this.filterAttendeeList(
				this.props.attendeeRecords,
				this.state.search.filter
			),
			this.state.search.query
		);

		return (
			<div>
				<div className="title-container">
					<Title className="title" level={3}>
						Session
					</Title>
					<span className="subtitle">
						{formatFullDateTimeString(this.props.activeSession!.startTime)}
					</span>
				</div>
				<Row>
					<Col span={8}>
						<Search
							className="search-input"
							placeholder="Search..."
							onSearch={value => this.searchBy(value)}
							enterButton
						/>
					</Col>
					<Col span={8} offset={8}>
						<div>
							<span className="order-by-sub">Filter:</span>
							<Select
								className="order-by-select"
								defaultValue={FilterBy.ALL}
								onChange={(value: any) => this.filterBy(value)}
							>
								<Select.Option value={FilterBy.ALL}>
									All attendees
								</Select.Option>
								<Select.Option value={FilterBy.PRESENT}>
									Present attendees
								</Select.Option>
								<Select.Option value={FilterBy.ABSENT}>
									Absent attendees
								</Select.Option>
								<Select.Option value={FilterBy.NOT_YET}>
									Undetermined
								</Select.Option>
							</Select>
						</div>
					</Col>
				</Row>
				<div
					className={classNames({
						'attendee-container': true,
						'is-loading': this.props.isLoadingAttendeeRecords
					})}
				>
					{this.props.isLoadingAttendeeRecords ? (
						<Spin size="large" />
					) : (
							<Table
								columns={columns}
								dataSource={processedList}
								pagination={false}
								rowKey={record => record.attendee.id.toString()}
							/>
						)}
				</div>
			</div>
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
