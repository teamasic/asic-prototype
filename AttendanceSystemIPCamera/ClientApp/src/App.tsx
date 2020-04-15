import * as React from 'react';
import { connect } from 'react-redux';
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
import { ApplicationState } from './store';
import { UserState } from './store/user/userState';
import User from './models/User';

import { userActionCreators } from './store/user/userActionCreators';
import { bindActionCreators } from 'redux';

type AppProps=
	UserState &
	typeof userActionCreators;

class AppComponent extends React.Component<AppProps> {

	constructor(props: any) {
		super(props);
	}

	componentDidMount() {
		if (!this.props.isLogin) {
			this.props.checkUserInfo();
		}
	}

	public render() {
		if (!this.props.successfullyLoaded) {
			return (
				<Layout>
				</Layout>
			);
		}
		if (this.props.isLogin) {
			console.log(this.props.currentUser);
			return (
				<Layout >
					<Route exact path='/'>
						<Redirect exact to='/dashboard' />
					</Route>
					<Route exact path="/dashboard" component={Dashboard} />
					<Route exact path="/session/:id" component={Session} />
					<Route exact path="/group/session/:id" component={Session} />
					<Route exact path="/group/:code" component={GroupDetail} />
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

const matchDispatchToProps = (dispatch: any) => {
	return bindActionCreators(userActionCreators, dispatch);
  }

export default connect((state: ApplicationState) => state.user, matchDispatchToProps)(AppComponent);