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
	page: number
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
			page: 1
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
	
	private renderOnRow = (record: any, index: number) => {
		if (index % 2 == 0) {
			return 'default';
		} else {
			return 'striped';
		}
	}

	public render() {
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
						onChange={() => this.props.markAsPresent(pair.attendee.id)}>
					</Radio>
			},
			{
				title: 'Absent',
				key: 'absent',
				width: '20%',
				render: (text: string, pair: AttendeeRecordPair) =>
					<Radio
						checked={pair.record != null && !pair.record.present}
						onChange={() => this.props.markAsAbsent(pair.attendee.id)}></Radio>
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
					<Col>
						<div className="row centered">
							<Button type="primary"
								className="take-attendance-button"
								disabled={true}
								onClick={this.openModelTakingAttendance}>
								Start taking attendance
							</Button>
							<Badge color={"orange"} text="Currently taking attendance" />
						</div>
					</Col>
				</Row>
				{
					<TakeAttendanceModal
						visible={this.state.isModelOpen}
						sessionId={this.props.sessionId}
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
								pagination={false}
								rowKey={record => record.attendee.id.toString()}
								rowClassName={this.renderOnRow}
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
