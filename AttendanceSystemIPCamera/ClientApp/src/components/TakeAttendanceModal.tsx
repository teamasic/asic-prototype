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
const { Search } = Input;
const { Title } = Typography;

interface Props {
	visible: boolean;
	sessionId: number;
	onClose: () => void;
}

// At runtime, Redux will merge together...
type ModalProps = Props & SessionState & // ... state we've requested from the Redux store
	typeof sessionActionCreators // ... plus action creators we've requested
	& RouteComponentProps<{
		id?: string;
	}>; // ... plus incoming routing parameters

interface TakeAttendanceState {
	startTime: moment.Moment,
	endTime: moment.Moment,
	isError: boolean
}

class TakeAttendanceModal extends React.PureComponent<ModalProps, TakeAttendanceState> {
	public constructor(props: ModalProps) {
		super(props);
		this.state = {
			startTime: moment(),
			endTime: moment().add(1, "m"),
			isError: false,
		};
	}

	private onOkModel = async () => {
		let startTime = this.state.startTime;
		let endTime = this.state.endTime;
		if (startTime.isSameOrAfter(endTime) || minutesOfDay(startTime) < minutesOfDay(moment())){
			error("Start time and end time is not suitable")
		}
		else {
			// this.props.startRealTimeConnection();
			this.props.onClose();
			if (this.props.activeSession != null) {
				this.props.startTakingAttendance(this.props.activeSession);
			}
			const data = await takeAttendance({
				sessionId: this.props.sessionId,
				startTime: this.state.startTime.format('YYYY-MM-DD HH:mm'),
				endTime: this.state.endTime.format('YYYY-MM-DD HH:mm'),
			})
			if (data.success == false){
				error("Error while taking attendance, please try again")
			}
			this.props.startTakingAttendance(null);
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
				<Modal
				title="Start taking attendance"
				visible={this.props.visible}
				onCancel={() => this.props.onClose()}
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
		);
	}
}

export default connect(
	(state: ApplicationState, ownProps: Props) => ({
		...state.sessions,
		...ownProps
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(TakeAttendanceModal as any);
