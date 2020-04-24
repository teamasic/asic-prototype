import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { sessionActionCreators } from '../store/session/actionCreators';
import {
	Button,
	Col,
	Row,
	Modal,
	Input,
	Badge} from 'antd';
import { Typography } from 'antd';
import '../styles/Session.css';
import { SessionState } from '../store/session/state';
import '../styles/ActiveView.css';
import PresentSection from './PresentSection';
import UnknownSection from './UnknownSection';

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
				<PresentSection sessionId={this.props.sessionId} 
				markAsAbsent={this.props.markAsAbsent}  />

				<UnknownSection
					editable={true}
					sessionId={this.props.sessionId} 
					markAsPresent={this.props.markAsPresent} />
				
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
