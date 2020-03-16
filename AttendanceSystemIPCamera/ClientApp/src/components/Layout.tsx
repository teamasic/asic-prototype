import * as React from 'react';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { Container } from 'reactstrap';
import { connect } from 'react-redux';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Badge } from 'antd';
import classNames from 'classnames';
import '../styles/Layout.css';
import { Link, withRouter } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';
import { ChangeRequestState } from '../store/changeRequest/state';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { sessionActionCreators } from '../store/session/actionCreators';
import { ChangeRequestStatusFilter } from '../models/ChangeRequest';

const { Header, Sider, Content, Footer } = Layout;

// At runtime, Redux will merge together...
type LayoutProps = 
	ChangeRequestState & // ... state we've requested from the Redux store
	typeof changeRequestActionCreators &
	typeof sessionActionCreators &
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
		selectedKeys: ['groups']
	};

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	componentDidMount() {
		this.props.requestActiveSession();
		if (!this.props.successfullyLoaded) {
			this.props.requestChangeRequests(ChangeRequestStatusFilter.UNRESOLVED);
		}
		const nonDefaultPathnames = ['change-requests'];
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
		return (
			<Layout className="layout">
				<Sider
					className="sider"
					collapsible
					collapsed={this.state.collapsed}
					onCollapse={this.onCollapse}
				>
					<div className="logo">ASIC</div>
					<Menu theme="dark"
						selectedKeys={this.state.selectedKeys}
						onSelect={item => this.setState({
							selectedKeys: [item.key]
						})}
						mode="inline">
						<Menu.Item key="groups">
							<Icon type="hdd" />
							<div className="link-container">
								<Link to="/">
									Your groups
								</Link>
							</div>
						</Menu.Item>
						<Menu.Item key="change-requests">
							<Icon type="file-exclamation" />
							<div className="link-container">
								<Link to="/change-requests">
									Change requests
								</Link>
								<Badge count={this.props.unresolvedCount} showZero={false}
									className="borderless-badge" style={{
									marginLeft: '5px'
								}}>
									</Badge>
							</div>
						</Menu.Item>
						<Menu.Item key="3">
							<Icon type="sync" />
							<span>Sync</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout className={classNames({
					'inner-layout': true,
					'with-sidebar-collapsed': this.state.collapsed
				})}>
					<Content className="content">
						{this.props.children}
					</Content>
				</Layout>
			</Layout>
		);
	}
}

// export default PageLayout;


export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.changeRequests
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
		...changeRequestActionCreators,
		...sessionActionCreators
	}, dispatch) // Selects which action creators are merged into the component's props
)(PageLayout as any));