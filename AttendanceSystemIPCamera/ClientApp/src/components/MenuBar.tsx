import * as React from 'react';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import NavMenu from './NavMenu';
import '../styles/MenuBar.css';
import { Layout, Menu, Breadcrumb, Icon, Dropdown, Badge, Row, Col, Spin, Avatar, Button } from 'antd';
import classNames from 'classnames';
import { Link, withRouter } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';
import { notificationActionCreators } from '../store/notification/actionCreators';
import { constants } from '../constant';
import * as firebase from '../firebase';
import { UserState } from '../store/user/userState';
import { userActionCreators } from '../store/user/userActionCreators';
import { NotificationState } from '../store/notification/state';
import Notification from './Notification';
const { Header, Sider, Content, Footer } = Layout;

// At runtime, Redux will merge together...
type Props =
	UserState &
	NotificationState &
	typeof notificationActionCreators &
	typeof userActionCreators &
	RouteComponentProps<{}>; // ... plus incoming routing parameters


class MenuBar extends React.Component<Props> {
	render() {
		const notificationCount = this.props.notifications.filter(n => !n.read).length;
		return <Content className="menu-bar row">
				<div className="fullname">{this.props.currentUser.name}</div>
				<Avatar className="avatar" src={this.props.currentUser.image} />
				<Dropdown overlay={this.renderNotificationMenu()} trigger={['click']}>
					<Badge count={notificationCount}>
					<Icon type="bell" className="notification-icon" />
					</Badge>
				</Dropdown>
			</Content>;
	}

	private renderNotificationMenu() {
		if (this.props.notifications.length === 0) {
			return (
				<Menu className="no-notification-menu">
					<Menu.Item>
						<div className="no-notification">No notifications</div>
					</Menu.Item>
				</Menu>
			);
		}
		return (
			<Menu>
				<div className="notification-header">
					<Button type="link" size="small" onClick={() => this.props.markAllAsRead()}>Mark all as read</Button>
				</div>
				{
					this.props.notifications.map((notification, i) => (
						<Menu.Item key={notification.id}>
							<Notification
								notification={notification}
								markAsRead={() => this.props.markAsRead(notification.id)}
								lastChild={i === this.props.notifications.length - 1} />
						</Menu.Item>
					))
				}
			</Menu>
		);
	}
}

export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.user,
		...state.notifications
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
		...notificationActionCreators
	}, dispatch) // Selects which action creators are merged into the component's props
)(MenuBar as any));