import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Dashboard from './components/Dashboard';
import Session from './components/Session';

import './App.css';

export default () => (
	<Layout>
		<Route exact path="/" component={Dashboard} />
		<Route exact path="/session" component={Session} />
	</Layout>
);
