import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import { Link, withRouter } from 'react-router-dom';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Breadcrumb, Icon, Button, Empty, message, Typography, Tabs, Row, Col, InputNumber } from 'antd';
import GroupInfo from './GroupInfo';
import PastSession from './PastSession';
import ModalExport from './ModalExport';
import { roomActionCreators, requestRooms } from '../store/room/actionCreators';
import { RoomsState } from '../store/room/state';
import { UnitsState } from '../store/unit/state';
import { sessionActionCreators } from '../store/session/actionCreators';
import { unitActionCreators } from '../store/unit/actionCreators';
import classNames from 'classnames';
import { EditTwoTone } from '@ant-design/icons';
import TopBar from './TopBar';
import StartSessionModal from './StartSessionModal';

const { Title } = Typography;
const { Paragraph } = Typography
const { TabPane } = Tabs;

interface GroupDetailComponentState {
    modalExportVisible: boolean,
    attendeeLoading: boolean,
    editMaxSession: boolean,
    modalStartSessionVisible: boolean
}

// At runtime, Redux will merge together...
type GroupDetailProps =
    GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & UnitsState
    & RoomsState// ... state we've requested from the Redux store
    & typeof roomActionCreators
    & typeof unitActionCreators
    & typeof sessionActionCreators
    & RouteComponentProps<{
        id?: string;
    }>; // ... plus incoming routing parameters


class GroupDetail extends React.PureComponent<GroupDetailProps, GroupDetailComponentState> {
    state = {
        modalExportVisible: false,
        attendeeLoading: true,
        editMaxSession: false,
        modalStartSessionVisible: false
    }
    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public editGroupName = (str: string) => {
        var group = {
            ...this.props.selectedGroup,
            name: str
        }
        this.props.startUpdateGroup(group, this.updateGroupSuccess);
    }

    public updateGroupSuccess() {
        message.success("Update group name success!")
    }

    public openModalExport = () => {
        this.setState({
            modalExportVisible: true
        })
    }

    public openModalStartSession = () => {
        this.setState({
            modalStartSessionVisible: true
        })
    }

    public closeModalExport = () => {
        this.setState({
            modalExportVisible: false
        })
    }

    public closeModalStartSession = () => {
        this.setState({
            modalStartSessionVisible: false
        })
    }

    private redirect(url: string) {
        this.props.history.replace(url);
    }

    private onEditMaxSession = () => {
        this.setState({
            editMaxSession: true
        });
    }

    private onMaxSessionBlur = (e: any) => {
        var group = {
            ...this.props.selectedGroup,
            maxSessionCount: JSON.parse(e.target.value)
        };
        this.props.startUpdateGroup(group, this.updateGroupSuccess);
        this.setState({
            editMaxSession: false
        });
    }

    public render() {
        const tabBarExtra =
            <div className="tab-bar-extra">
                <Button type="default" onClick={this.openModalExport}
                    icon="export">Export</Button>
                <Button type="default" onClick={this.openModalStartSession}
                    icon="plus">Create a session</Button>
            </div>;
        return (
            <React.Fragment>
                <TopBar>
                    <Breadcrumb.Item>
                        <span>{this.props.selectedGroup.code} - {this.props.selectedGroup.name}</span>
                    </Breadcrumb.Item>
                </TopBar>
                <div className="title-container">
                    <Row>
                        <Col>
                            <Title className="title" level={3}>
                                <Paragraph editable={{ onChange: this.editGroupName }}>{this.props.selectedGroup.name}</Paragraph>
                            </Title>
                        </Col>
                        <Col>
                            <Title className="title" level={4}>
                                <Paragraph>Total sessions : {this.state.editMaxSession ?
                                    (<InputNumber
                                        defaultValue={this.props.selectedGroup.maxSessionCount}
                                        min={0}
                                        max={100}
                                        onBlur={this.onMaxSessionBlur}
                                    />
                                    ) :
                                    (
                                        <span>{this.props.selectedGroup.maxSessionCount} < EditTwoTone onClick={this.onEditMaxSession} /></span>
                                    )
                                }</Paragraph>
                            </Title>
                        </Col>
                    </Row>
                </div>
                <Tabs defaultActiveKey="1" type="card"
                    tabBarExtraContent={tabBarExtra}>
                    <TabPane tab="Group Information" key="1">
                        <GroupInfo attendees={this.props.selectedGroup.attendees} attendeeLoading={this.state.attendeeLoading} />
                    </TabPane>
                    <TabPane tab="Past Session" key="2">
                        <PastSession group={this.props.selectedGroup} redirect={url => this.redirect(url)} />
                    </TabPane>
                </Tabs>
                <ModalExport modalVisible={this.state.modalExportVisible}
                    group={this.props.selectedGroup}
                    closeModal={this.closeModalExport} />
                <StartSessionModal
                    group={this.props.selectedGroup}
                    hideModal={() => this.closeModalStartSession()}
                    modelOpen={this.state.modalStartSessionVisible}
                    redirect={url => this.redirect(url)}
                    roomList={this.props.roomList}
                    units={this.props.units}
                />
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        var strId = this.props.match.params.id;
        if (strId) {
            var intId = parseInt(strId);
            this.props.requestGroupDetail(intId, () => {
                this.setState({
                    attendeeLoading: false
                });
            });
            this.props.requestRooms();
            this.props.requestActiveSession();
            this.props.requestUnits();
        }
    }
}

export default withRouter(connect(
    (state: ApplicationState) => ({
        ...state.groups, ...state.rooms, ...state.units
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators({
        ...roomActionCreators,
        ...groupActionCreators,
        ...sessionActionCreators,
        ...unitActionCreators
    }, dispatch) // Selects which action creators are merged into the component's props
)(GroupDetail as any));
