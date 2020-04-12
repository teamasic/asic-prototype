import * as React from 'react';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Layout, Menu, Breadcrumb, Icon, Dropdown, Badge, Row, Col, Spin, Avatar, Button } from 'antd';
import classNames from 'classnames';
import { Link, withRouter } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';
import { constants } from '../constant';
import * as firebase from '../firebase';
import Notification, { NotificationType } from '../models/Notification';
import { SessionNotificationViewModel } from '../models/SessionViewModel';
import { formatDateDistanceToNow, formatTimeOnly } from '../utils';
const { Header, Sider, Content, Footer } = Layout;

interface Props {
	notification: Notification;
	markAsRead: () => void;
	lastChild: boolean;
}

// At runtime, Redux will merge together...
type NotificationProps =
	Props &
	RouteComponentProps<{}>; // ... plus incoming routing parameters


class NotificationComponent extends React.Component<NotificationProps> {
	render() {
		let content: any;
		switch (this.props.notification.type) {
			case NotificationType.SESSION:
				content = this.renderSessionNotification();
				break;
			default:
				content = <div></div>;
		}
		return <div className={classNames({
			'notification': true,
			'unread': !this.props.notification.read,
			'last-child': this.props.lastChild
		})}>
			{
				content
			}
			<div className="time">
				<Icon className="icon" type="clock-circle" />
				{formatDateDistanceToNow(this.props.notification.timeSent)}
			</div>
		</div>;
	}

	private renderSessionNotification() {
		const session = this.props.notification.data as SessionNotificationViewModel;
		return <div className="session" onClick={() => {
			this.props.markAsRead();
			this.props.history.push(`/session/${session.id}`);
		}}>
			<div className="title-box">
				<div className="session-name">{session.groupName}</div>
			</div>
			<div className="session-info">
				{formatTimeOnly(session.startTime)}-{formatTimeOnly(session.endTime)} - Room: {session.roomName}
			</div>
		</div>
	}
}

export default withRouter(NotificationComponent);