import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router';
import { createBrowserHistory } from 'history';
import configureStore from './store/configureStore';
import App from './App';
import registerServiceWorker from './registerServiceWorker';
import createSignalRConnection from './middlewares/SignalRConnection';
import { attachEvents } from './middlewares/SignalRMiddleware';
import { config } from "dotenv"
import { resolve } from "path"
import ErrorBoundary from 'react-error-boundary';
import ErrorDisplay from './ErrorDisplay';

config({ path: resolve(__dirname, "../.env") })

// Create browser history to use in the Redux store
const baseUrl = document
	.getElementsByTagName('base')[0]
	.getAttribute('href') as string;
const history = createBrowserHistory({ basename: baseUrl });

// Get the application-wide store instance, prepopulating with state from the server where available.
const store = configureStore(history);
const connection = createSignalRConnection();
attachEvents(connection, store.dispatch);
connection.start().catch(err => console.log(err));

ReactDOM.render(
	<Provider store={store}>
		<ConnectedRouter history={history}>
			<ErrorBoundary FallbackComponent={ErrorDisplay}>
				<App />
			</ErrorBoundary>
		</ConnectedRouter>
	</Provider>,
	document.getElementById('root')
);

registerServiceWorker();
