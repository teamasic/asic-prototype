import * as React from 'react';
import { Route, Redirect } from 'react-router';
import Layout from './components/Layout';
import Dashboard from './components/Dashboard';
import Session from './components/Session';
import GroupDetail from './components/GroupDetail'

import './App.css';
import ChangeRequests from './components/ChangeRequests';
import Settings from './components/Settings';
import { constants } from './constant';
import Login from './components/Login';

class AppComponent extends React.Component {

	constructor(props: any) {
		super(props);
	}

	public render() {
		const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
		if (authData) {
			return (
				<Layout >
					<Route exact path='/'>
						<Redirect exact to='/dashboard' />
					</Route>
					<Route exact path="/dashboard" component={Dashboard} />
					<Route exact path="/session/:id" component={Session} />
					<Route exact path="/group/session/:id" component={Session} />
					<Route exact path="/group/:id" component={GroupDetail} />
					<Route exact path="/change-requests" component={ChangeRequests} />
					<Route exact path="/settings" component={Settings} />
				</Layout>
			);
		} else {
			return (
				<Layout >
					<Redirect exact to='/' />
					<Route exact path="/" component={Login} />
				</Layout>
			);
		}
	}
}


export default AppComponent;
