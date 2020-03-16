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
import { formatFullDateTimeString, minutesOfDay } from '../utils';
import { takeAttendance } from '../services/session';
import moment from 'moment';
import '../styles/Table.css';
import TopBar from './TopBar';
const { Search } = Input;
const { Title } = Typography;

// At runtime, Redux will merge together...
type SessionProps = SessionState & // ... state we've requested from the Redux store
	typeof sessionActionCreators // ... plus action creators we've requested
	& RouteComponentProps<{
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
	isModelOpen: boolean,
	startTime: moment.Moment,
	endTime: moment.Moment,
	isError: boolean,
	page: number
}

class Session extends React.PureComponent<SessionProps, SessionLocalState> {
	public constructor(props: SessionProps) {
		super(props);
		this.state = {
			sessionId: 0,
			search: {
				query: '',
				filter: FilterBy.ALL
			},
			isModelOpen: false,
			startTime: moment(),
			endTime: moment().add(1, "m"),
			isError: false,
			page: 1
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
	private openModelTakingAttendance = () => {
		this.setState({
			isModelOpen: true,
			startTime: moment(),
			endTime: moment().add(1, "m"),
			isError: false
		})
	}
	private onCancelModel = () => {
		this.setState({
			isModelOpen: false,
		})
	}
	
	private onOkModel = async () => {
		let startTime = this.state.startTime;
		let endTime = this.state.endTime;
		if (startTime.isSameOrAfter(endTime) || minutesOfDay(startTime) < minutesOfDay(moment())){
			this.setState({
				isError: true,
			})
		}
		else {
			// this.props.startRealTimeConnection();
			const data = await takeAttendance({
				sessionId: this.state.sessionId,
				startTime: this.state.startTime.format('YYYY-MM-DD HH:mm'),
				endTime: this.state.endTime.format('YYYY-MM-DD HH:mm'),
			})
			this.setState({
				isModelOpen: false
			})
		}

	}
	private onChangeStartTime = (time: moment.Moment) => {
		this.setState({
			startTime: time,
		})
	}
	private onChangeEndTime = (time: moment.Moment) => {
		this.setState({
			endTime: time
		})
	}
	private getDisableHours = () => {
		let hours = [];
		for (var i = 0; i < moment().hour(); i++) {
			hours.push(i);
		}
		return hours;
	}
	private getDisableMinutes = () => {
		let minutes = []
		let currentHour = moment().hour();
		let startTimeHour = this.state.startTime.hour();
		if (currentHour == startTimeHour) {
			let currentMinute = moment().minute();
			for (let i = 0; i < currentMinute; i++) {
				minutes.push(i);
			}
		}
		return minutes;
	}
	public render() {
		return (
			<React.Fragment>
				<TopBar>
					{
						this.props.activeSession &&
						<Breadcrumb.Item>
							<Link to={`groups/${this.props.activeSession.groupId}`}>
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

	private renderOnRow = (record: any, index: number) => {
		if (index % 2 == 0) {
			return 'default';
		} else {
			return 'striped';
		}
	}

	private renderSessionSection() {
		const columns = [
			{
				title: "#",
				key: "index",
				width: '5%',
				render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
			},
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
				width: '12%',
				render: (text: string, pair: AttendeeRecordPair) =>
					<Radio
					checked={pair.record != null && pair.record.present}
						onChange={() => this.markAsPresent(pair.attendee.id)}>
					</Radio>
			},
			{
				title: 'Absent',
				key: 'absent',
				width: '20%',
				render: (text: string, pair: AttendeeRecordPair) =>
					<Radio
						checked={pair.record != null && !pair.record.present}
						onChange={() => this.markAsAbsent(pair.attendee.id)}></Radio>
			}
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
				<Row style={{ marginTop: 5 }} type="flex" gutter={[4, 4]} align="bottom">
					<Col span={4}>
						<Button type="primary"
							onClick={this.openModelTakingAttendance}>
							Start taking attendance
							</Button>
					</Col>
					<Col span={12}>
						<Badge color={"orange"} text="Taking attendance until 20:45" />
					</Col>
				</Row>
				<Modal
					title="Start taking attendance"
					visible={this.state.isModelOpen}
					onCancel={this.onCancelModel}
					onOk={this.onOkModel} okText="Start">
					<Row justify="start" style={{ marginTop: 5 }} type="flex" align="middle" gutter={[0, 0]}>
						<Col span={12}>
							<span style={{ marginRight: 5 }}>Start time</span>
							<TimePicker onChange={this.onChangeStartTime} value={this.state.startTime} format="HH:mm" disabledHours={this.getDisableHours} disabledMinutes={this.getDisableMinutes} />
						</Col>
						<Col span={12}>
							<span style={{ marginRight: 5 }}>End time</span>
							<TimePicker onChange={this.onChangeEndTime} value={this.state.endTime} format="HH:mm" disabledHours={this.getDisableHours} disabledMinutes={this.getDisableMinutes} />
						</Col>
					</Row>
					{this.state.isError ? 
					<Row style={{marginTop: 15}} >
						<Col>
							<p style={{color: "red"}}>* Start time and end time is not suitable</p>
						</Col>
					</Row> : null}
				</Modal>
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
								bordered
								pagination={false}
								rowKey={record => record.attendee.id.toString()}
								rowClassName={this.renderOnRow}
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

export default withRouter(connect(
	(state: ApplicationState, ownProps: SessionProps) => ({
		...state.sessions,
		...ownProps
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(Session as any));
