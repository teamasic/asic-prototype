import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { Link, withRouter } from 'react-router-dom';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { Breadcrumb, Icon, Button, Empty, message, Typography, Tabs, Row, Col, InputNumber } from 'antd';
import { SettingState } from '../store/settings/state';
import { settingActionCreators } from '../store/settings/actionCreators';
import classNames from 'classnames';
import TopBar from './TopBar';
import Setting from '../models/Setting';
import '../styles/Settings.css';

const { Title } = Typography;
const { Paragraph } = Typography
const { TabPane } = Tabs;

// At runtime, Redux will merge together...
type SettingProps =
    SettingState &
    typeof settingActionCreators
    & RouteComponentProps<{}>; // ... plus incoming routing parameters


class Settings extends React.PureComponent<SettingProps> {
    state = {
    };

    // This method is called when the component is first added to the document
    public componentDidMount() {
    }

    private checkForUpdates() {
        this.props.checkForUpdates();
    }

    public render() {
        const updatableBoxes = ['Model', 'Room', 'Unit', 'Others'];
        return (
            <React.Fragment>
                <TopBar>
                    <Breadcrumb.Item>
                        <span>Settings</span>
                    </Breadcrumb.Item>
                </TopBar>
                <div className="title-container">
                    <Title className="title" level={3}>
                        Settings
                    </Title>
                </div>
                <div className="content-container">
                    <Button size="large"
                        type="primary"
                        onClick={() => this.checkForUpdates()}>Check for updates</Button>
                    {updatableBoxes.map(key =>
                        this.renderUpdateBox(key, (this.props as any)[key.toLowerCase()]))}
                </div>
            </React.Fragment>
        );
    }

    private renderUpdateBox(name: string, setting: Setting) {
        return <div className="update-box row centered" key={name}>
            <div>
                <Title level={3}>{name}</Title>
                Last updated: {new Date(setting.lastUpdated).toLocaleDateString()}
            </div>
            {
                setting.needsUpdate ?
                    <Button icon="exclamation-circle" type="default">Update now</Button> :
                    <div><Icon type="check-circle" />Updated</div>
            }
        </div>;
    }
}

export default withRouter(connect(
    (state: ApplicationState) => ({
        ...state.settings
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators({
        ...settingActionCreators
    }, dispatch) // Selects which action creators are merged into the component's props
)(Settings as any));
