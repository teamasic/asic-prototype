import * as React from 'react';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { Container } from 'reactstrap';
import { connect } from 'react-redux';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Badge } from 'antd';
import classNames from 'classnames';
import '../styles/Layout.css';
import { Link } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';
import { ChangeRequestState } from '../store/changeRequest/state';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { ChangeRequestStatusFilter } from '../models/ChangeRequest';

const { Header, Sider, Content, Footer } = Layout;

// At runtime, Redux will merge together...
type LayoutProps = 
	ChangeRequestState & // ... state we've requested from the Redux store
	typeof changeRequestActionCreators &
	RouteComponentProps<{}>; // ... plus incoming routing parameters

class PageLayout extends React.Component<
	LayoutProps,
	{
		collapsed: boolean;
	}
> {
	state = {
		collapsed: false
	};

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	componentDidMount() {
		if (!this.props.successfullyLoaded) {
			this.props.requestChangeRequests(ChangeRequestStatusFilter.UNRESOLVED);
		}
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
					<Menu theme="dark" defaultSelectedKeys={['1']} mode="inline">
						<Menu.Item key="1">
							<Icon type="hdd" />
							<div className="link-container">
								<Link to="/">
									Your groups
								</Link>
							</div>
						</Menu.Item>
						<Menu.Item key="2">
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


export default connect(
	(state: ApplicationState) => ({
		...state.changeRequests
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
		...changeRequestActionCreators
	}, dispatch) // Selects which action creators are merged into the component's props
)(PageLayout as any);