import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Dashboard from './components/Dashboard';
import Session from './components/Session';
import GroupDetail from './components/GroupDetail'

import './App.css';
import ChangeRequests from './components/ChangeRequests';

export default () => (
	<Layout>
		<Route exact path="/" component={Dashboard} />
		<Route exact path="/session/:id" component={Session} />
		<Route exact path="/group/session/:id" component={Session} />
		<Route exact path="/group/:id" component={GroupDetail} />
		<Route exact path="/change-requests" component={ChangeRequests} />
	</Layout>
);
