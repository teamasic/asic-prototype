import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Badge } from 'antd';
import classNames from 'classnames';
import '../styles/Layout.css';
import { Link } from 'react-router-dom';

const { Header, Sider, Content, Footer } = Layout;

class PageLayout extends React.Component<
	any,
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
								<Badge count={5} className="borderless-badge" style={{
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

export default PageLayout;
