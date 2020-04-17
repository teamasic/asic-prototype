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
	markAsPresent: (attendeeCode: string) => void;
	markAsAbsent: (attendeeCode: string, assumeSuccess: boolean) => void;
}

// At runtime, Redux will merge together...
type SessionProps = Props & SessionState & // ... state we've requested from the Redux store
	typeof sessionActionCreators // ... plus action creators we've requested
	& RouteComponentProps<{
		id?: string;
	}>; // ... plus incoming routing parameters

interface State {
	unknownModalVisible: boolean;
	currentUnknownImage: string;
}

class SessionActiveView extends React.PureComponent<SessionProps, State> {
	public constructor(props: SessionProps) {
		super(props);
		this.state = {
			unknownModalVisible: false,
			currentUnknownImage: ''
		};
	}

	private openUnknownModal(currentUnknownImage: string) {
		this.setState({
			unknownModalVisible: true,
			currentUnknownImage
		});
	}

	private closeUnknownModal() {
		this.setState({
			unknownModalVisible: false,
			currentUnknownImage: ''
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

	private confirmMarkAbsent(ar: AttendeeRecordPair) {
		confirm({
			title: "Do you want to mark this attendee as absent?",
			okType: "danger",
			onOk: () => {
				this.props.markAsAbsent(ar.attendee.code, false);
			},
		});
	}

	private removeImage(img: string) {
		confirm({
			title: "Do you want to remove this unknown attendee?",
			okType: "danger",
			onOk: () => {
				this.props.removeUnknownImage(img);
			}
		});
	}

	private getPresentSection() {
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
										{this.getImageBox(`url(/api/avatars/${ar.attendee.image})`)}
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

	private getUnknownSection() {
		const unknowns = this.props.unknownImages.slice().reverse();
		return <div className="container">
			<Title className="title" level={4}>Unknown attendees</Title>
			{
				unknowns.length > 0 ?
					<div className="box-container fixed-grid--around">
						{
							unknowns.map(img =>
								<div key={img}
									className="attendee-box grid-element">
									<div className="inner-box">
										{this.getImageBox(`url(/api/unknown/${img})`)}
										<div className="inner-box-actions">
											<Tooltip title="Remove this image">
												<Button
													onClick={() => this.removeImage(img)}
													size="small"
													type="danger"
													shape="circle"
													icon="close" />
											</Tooltip>
											<Tooltip title="Mark attendee present">
												<Button
													onClick={() => this.openUnknownModal(img)}
													className=""
													type="primary"
													size="small"
													shape="circle"
													icon="issues-close" />
											</Tooltip>
										</div>
									</div>
								</div>
							)
						}
					</div> :
					<Empty description="Unknown attendees will show up here." />
			}
			{
				<MarkUnknownPresentModal
					unknownImage={this.state.currentUnknownImage}
					removeUnknownImage={(img: string) => this.props.removeUnknownImage(img)}
					visible={this.state.unknownModalVisible}
					hideModal={() => this.closeUnknownModal()}
					markAsPresent={this.props.markAsPresent}
					attendeeRecords={this.props.attendeeRecords}
				/>
			}
		</div>;
	}

	public render() {
		return (
			<div>
				<Row style={{ marginTop: 5 }} type="flex" gutter={[4, 4]} align="bottom">
					<Col>
						<div className="row centered">
							<Button type="primary" disabled={true}
								className="take-attendance-button">
								Taking attendance
							</Button>
						</div>
					</Col>
					<Badge color={"orange"} text="Currently taking attendance" />
				</Row>
				{this.getPresentSection()}
				{this.getUnknownSection()}
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
