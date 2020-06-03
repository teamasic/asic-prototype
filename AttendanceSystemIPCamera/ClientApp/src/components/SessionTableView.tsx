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
import { formatFullDateTimeString, minutesOfDay, error, renderStripedTable, compareDate } from '../utils';
import { takeAttendance } from '../services/session';
import moment from 'moment';
import '../styles/Table.css';
import TopBar from './TopBar';
import TakeAttendanceModal from './TakeAttendanceModal';
import TableConstants from '../constants/TableConstants';
import SessionStatusConstants from '../constants/SessionStatusConstants';
const { Search } = Input;
const { Title } = Typography;

interface Props {
	sessionId: number;
	markAsPresent: (attendeeCode: string) => void;
	markAsAbsent: (attendeeCode: string) => void;
	isSessionEditable: boolean;
}

// At runtime, Redux will merge together...
type SessionProps = Props & SessionState & // ... state we've requested from the Redux store
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

interface State {
	search: {
		query: string;
		filter: FilterBy;
	};
	isModelOpen: boolean,
	startTime: moment.Moment,
	endTime: moment.Moment,
	isError: boolean,
	currentPage: number,
	pageSize: number
}

class SessionTableView extends React.PureComponent<SessionProps, State> {
	public constructor(props: SessionProps) {
		super(props);
		this.state = {
			search: {
				query: '',
				filter: FilterBy.ALL
			},
			isModelOpen: false,
			startTime: moment(),
			endTime: moment().add(1, "m"),
			isError: false,
			currentPage: 1,
			pageSize: TableConstants.defaultPageSize
		};
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

	private searchAttendeeList(
		attendeeRecords: AttendeeRecordPair[],
		query: string
	) {
		const lowercaseQuery = query.toLowerCase();
		return attendeeRecords.filter(
			ar => ar.attendee.name.toLowerCase().includes(lowercaseQuery) ||
				ar.attendee.code.toLowerCase().includes(lowercaseQuery)
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

	private onPageChange = (page: number) => {
		this.setState({ currentPage: page });
	}

	private onShowSizeChange = (current: number, pageSize: number) => {
		this.setState({
			pageSize: pageSize,
			currentPage: current
		});
	}

	private takeAttendanceMultiple = async () => {
		if (this.props.activeSession != null) {
			this.props.startTakingAttendance(this.props.activeSession);
		}
		const data = await takeAttendance({
			sessionId: this.props.sessionId,
			multiple: true
		});
		if (data.success == false) {
			if (this.props.activeSession) {
				error(`Error while taking attendance at room ${this.props.activeSession.room.name}, please try again`)
			}
			else {
				error("Error while taking attendance, please try again")
			}
		}
		this.props.startTakingAttendance(null);
		this.props.endTakingAttendance();
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
						disabled={this.props.activeSession!.status === SessionStatusConstants.FINISHED}
						onChange={() => this.props.markAsPresent(pair.attendee.code)}>
					</Radio>
			},
			{
				title: 'Absent',
				key: 'absent',
				width: '20%',
				render: (text: string, pair: AttendeeRecordPair) =>
					<Radio
						checked={pair.record != null && !pair.record.present}
						disabled={this.props.activeSession!.status === SessionStatusConstants.FINISHED}
						onChange={() => this.props.markAsAbsent(pair.attendee.code)}></Radio>
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
					{this.props.isSessionEditable ?
						<Col>
							<div className="row centered">
								<Button type="primary"
									className="take-attendance-button"
									onClick={this.openModelTakingAttendance}>
									Start taking attendance
							</Button>
								<span className="or-separator"> or </span>
								<Button
									className="take-attendance-multiple-button"
									onClick={this.takeAttendanceMultiple}>
									Take attendance of multiple at once
							</Button>
							</div>
						</Col>
						: <div></div>
					}

				</Row>
				{
					<TakeAttendanceModal
						visible={this.state.isModelOpen}
						sessionId={this.props.sessionId}
						onChangeStartTime={this.onChangeStartTime}
						onChangeEndTime={this.onChangeEndTime}
						startTime={this.state.startTime}
						endTime={this.state.endTime}
						onClose={() => this.onCancelModel()} />
				}
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
								rowKey={record => record.attendee.code}
								pagination={{
									pageSize: this.state.pageSize,
									total: processedList != undefined ? processedList.length : 0,
									showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} attendees`,
									onChange: this.onPageChange,
									showSizeChanger: true,
									onShowSizeChange: this.onShowSizeChange
								}}
								rowClassName={renderStripedTable}
							/>
						)}
				</div>
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
)(SessionTableView as any);
