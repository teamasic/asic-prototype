import * as React from 'react';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Dropdown, Badge, Row, Col, Spin, Avatar, Button } from 'antd';
import classNames from 'classnames';
import '../styles/Layout.css';
import { Link, withRouter } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';
import { ChangeRequestState } from '../store/changeRequest/state';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { sessionActionCreators } from '../store/session/actionCreators';
import { ChangeRequestStatusFilter } from '../models/ChangeRequest';
import { constants } from '../constant';
import * as firebase from '../firebase';
import { UserState } from '../store/user/userState';
import { userActionCreators } from '../store/user/userActionCreators';
import { NotificationState } from '../store/notification/state';
import MenuBar from './MenuBar';
const { Header, Sider, Content, Footer } = Layout;

// At runtime, Redux will merge together...
type LayoutProps =
	UserState &
	ChangeRequestState & // ... state we've requested from the Redux store
	NotificationState &
	typeof changeRequestActionCreators &
	typeof sessionActionCreators &
	typeof userActionCreators &
	RouteComponentProps<{}>; // ... plus incoming routing parameters


class PageLayout extends React.Component<
	LayoutProps,
	{
		collapsed: boolean;
		selectedKeys: string[];
	}
	> {
	state = {
		collapsed: false,
		selectedKeys: ['groups'],
	};

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	componentDidMount() {
		this.props.requestActiveSession();
		if (!this.props.successfullyLoaded) {
			this.props.requestChangeRequests(ChangeRequestStatusFilter.UNRESOLVED);
		}
		const nonDefaultPathnames = ['change-requests', 'settings'];
		const currentPath = this.props.location.pathname.substring(1);
		nonDefaultPathnames.forEach(path => {
			if (currentPath.includes(path)) {
				this.setState({
					selectedKeys: [path]
				});
			}
		})
	}

	render() {
		console.log(this.props.history.location.pathname)
		return (<>{this.props.isLogin ? this.renderLayout() : this.renderEmty()}</>);
	}

	private renderEmty() {
		return (
			<Layout className="layout">
				<Row type='flex' align='middle' justify='space-around' >
					<Col span={8} >
						{this.props.children}
					</Col>
				</Row>
			</Layout>
		);
	}

	renderLayout() {
		return (
			<Layout className="layout">
				<Sider
					className="sider"
					collapsible
					collapsed={this.state.collapsed}
					onCollapse={this.onCollapse}
				>
					<div className="logo" >
						<img
							src="asic_logo.jpg"
							style={{
								backgroundSize: 'contain',
								backgroundPosition: 'center',
								borderRadius: '5px'
							}} />
					</div>
					<Menu theme="dark"
						selectedKeys={this.state.selectedKeys}
						onSelect={item => this.setState({
							selectedKeys: [item.key]
						})}
						mode="inline">
						<Menu.Item key="groups">
							<Icon type="team" />
							<span className="link-container">
								<Link to="/">
									Your groups
								</Link>
							</span>
						</Menu.Item>
						<Menu.Item key="change-requests">
							<Icon type="carry-out" />
							<span className="link-container">
								<Link to="/change-requests">
									Change requests
								</Link>
								<Badge count={this.props.unresolvedCount} showZero={false}
									className="borderless-badge" style={{
										marginLeft: '5px'
									}}>
								</Badge>
							</span>
						</Menu.Item>
						<Menu.Item key="settings">
							<Icon type="cloud-sync" />
							<span className="link-container">
								<Link to="/settings">
									Settings
								</Link>
							</span>
						</Menu.Item>
						<Menu.Item key="logout" onClick={() => this.logout()}>
							<Icon type="logout" />
							<span className="link-container">Logout</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout className={classNames({
					'inner-layout': true,
					'with-sidebar-collapsed': this.state.collapsed
				})}>
					<MenuBar />
					<Content className="content">
						{this.props.children}
					</Content>
				</Layout>
			</Layout>
		);
	}

	private logout() {
		if (this.props.isLogin) {
			firebase.auth.doSignOut().then(() => {
				localStorage.removeItem(constants.AUTH_IN_LOCAL_STORAGE);
				this.props.logout();
				window.location.href = "/";
			});
		}
	}
}

// export default PageLayout;


export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.changeRequests,
		...state.user,
		...state.notifications
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
		...changeRequestActionCreators,
		...sessionActionCreators,
		...userActionCreators
	}, dispatch) // Selects which action creators are merged into the component's props
)(PageLayout as any));