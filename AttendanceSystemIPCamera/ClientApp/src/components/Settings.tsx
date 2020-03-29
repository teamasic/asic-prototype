import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { Link, withRouter } from 'react-router-dom';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { Breadcrumb, Icon, Button, Empty, message, Typography, Tabs, Row, Col, InputNumber, Card } from 'antd';
import { SettingState } from '../store/settings/state';
import { settingActionCreators } from '../store/settings/actionCreators';
import classNames from 'classnames';
import TopBar from './TopBar';
import Setting from '../models/Setting';
import '../styles/Settings.css';
import { success, error } from '../utils'

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
        this.props.checkForUpdates(() => { });
    }

    private checkForUpdates() {
        this.props.checkForUpdates(() => {
            error('Unable to check for updates. Please check your internet connection and try again.');
        });
    }

    public render() {
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
                    {
                        this.renderUpdateBox("model", "Update facial recognition data",
                            "Update the data required for facial recognition tasks.",
                            this.props.model)
                    }
                    {
                        this.renderUpdateBox("room", "Update room configuration",
                            "Update your institution's room list and associated IP cameras.",
                            this.props.room)
                    }
                    {
                        this.renderUpdateBox("unit", "Update unit configuration",
                            "Update your institution's unit configuration (i.e. class periods, work hours, etc.)",
                            this.props.unit)
                    }
                    {
                        this.renderUpdateBox("others", "Update other settings",
                            "Update other miscellaneous settings.",
                            this.props.others)
                    }
                </div>
            </React.Fragment>
        );
    }

    private renderUpdateBox(key: string, name: string, description: string, setting: Setting) {
        return <Card className="update-card" title={name} bordered={false}>
            <Row>
                <Col span={20}>
                    <div>{description}</div>
                    <div>Last updated: {(new Date(setting.lastUpdated)).toLocaleDateString()}</div>
                </Col>
                <Col span={4}>
                    {
                        setting.needsUpdate ?
                            <Button loading={setting.loading} onClick={() => {
                                this.props.update(key, this.updateSuccess, this.updateError)
                            }} icon="sync">Update now</Button> :
                            <Button disabled={true} icon="sync">Already updated</Button>
                    }
                </Col>
            </Row>
        </Card>;
    }

    private updateSuccess() {
        success('Successfully updated.');
    }

    private updateError() {
        error('An error happened during the updating process. Please try again.');
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
