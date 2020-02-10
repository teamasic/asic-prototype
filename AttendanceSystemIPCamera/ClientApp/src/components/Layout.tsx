import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon } from 'antd';
import '../styles/Layout.css';

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
							<span>Your groups</span>
						</Menu.Item>
						<Menu.Item key="2">
							<Icon type="sync" />
							<span>Sync</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout>
					<Content className="content">
						{this.props.children}
					</Content>
				</Layout>
			</Layout>
		);
	}
}

export default PageLayout;
