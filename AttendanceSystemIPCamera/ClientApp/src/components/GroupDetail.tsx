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
import { Breadcrumb, Icon, Button, Empty, message, Typography, Tabs, Row, Col, InputNumber, Form, Input } from 'antd';
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
import { FormComponentProps } from 'antd/lib/form';
import Schedule from './Schedule';

const { Title } = Typography;
const { Paragraph } = Typography
const { TabPane } = Tabs;

interface GroupDetailComponentState {
    modalExportVisible: boolean,
    attendeeLoading: boolean,
    editMaxSession: boolean,
    modalStartSessionVisible: boolean,
    editGroupName: boolean
}

// At runtime, Redux will merge together...
type GroupDetailProps =
    FormComponentProps
    & GroupsState // ... state we've requested from the Redux store
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
    constructor(props: GroupDetailProps) {
        super(props);
        this.state = {
            modalExportVisible: false,
            attendeeLoading: true,
            editMaxSession: false,
            modalStartSessionVisible: false,
            editGroupName: false
        }
    }

    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public editGroupName = (e: any) => {
        this.props.form.validateFields((err: any, values: any) => {
            if(!err) {
                var group = {
                    ...this.props.selectedGroup,
                    name: values.name
                }
                this.props.startUpdateGroup(group, () => {this.setState({editGroupName: false})});
            }
        });
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
        this.setState({editMaxSession: true});
    }

    private onEditGroupName = () => {
        this.setState({editGroupName: true});
    }

    private onMaxSessionBlur = (e: any) => {
        this.props.form.validateFields((err: any, values: any) => {
            if(!err) {
                var group = {
                    ...this.props.selectedGroup,
                    maxSessionCount: JSON.parse(values.maxSession)
                };
                this.props.startUpdateGroup(group, () => {this.setState({editMaxSession: false})});
            }
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
        const { getFieldDecorator } = this.props.form;
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
                                {this.state.editGroupName ?
                                    (
                                        <Form>
                                            <Form.Item>
                                                {getFieldDecorator('name', {
                                                    initialValue: this.props.selectedGroup.name,
                                                    rules: [
                                                        { required: true, message: 'Please input group name' },
                                                        { min: 3, max: 50, message: 'Group name must have 3-50 characters'}
                                                    ],
                                                })(
                                                    <Input onBlur={this.editGroupName}/>
                                                )}
                                            </Form.Item>
                                        </Form>
                                    ) :
                                    (
                                        <span>{this.props.selectedGroup.name} < EditTwoTone onClick={this.onEditGroupName} /></span>
                                    )
                                }
                            </Title>
                        </Col>
                        <Col>
                        <Title className="title" level={4}>
                                {this.state.editMaxSession ?
                                    (
                                        <Form>
                                            <Form.Item>
                                                {getFieldDecorator('maxSession', {
                                                    initialValue: this.props.selectedGroup.maxSessionCount,
                                                    rules: [
                                                        { required: true, message: 'Please input max session' }
                                                    ],
                                                })(
                                                    <InputNumber
                                                        min={0}
                                                        max={100}
                                                        onBlur={this.onMaxSessionBlur}
                                                    />
                                                )}
                                            </Form.Item>
                                        </Form>
                                    ) :
                                    (
                                        <span>Total sessions: {this.props.selectedGroup.maxSessionCount} < EditTwoTone onClick={this.onEditMaxSession} /></span>
                                    )
                                }
                            </Title>
                        </Col>
                    </Row>
                </div>
                <Tabs defaultActiveKey="1" type="card"
                    tabBarExtraContent={tabBarExtra}>
                    <TabPane tab="Group Information" key="1">
                        <GroupInfo attendees={this.props.selectedGroup.attendees} attendeeLoading={this.state.attendeeLoading} />
                    </TabPane>
                    <TabPane tab="Schedule" key="2">
                        <Schedule groupId={this.props.selectedGroup.id}/>
                    </TabPane>
                    <TabPane tab="Past Session" key="3">
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

export default Form.create<GroupDetailProps>({ name: 'group_detail_form' })
    (withRouter(connect(
        (state: ApplicationState) => ({
            ...state.groups, ...state.rooms, ...state.units
        }), // Selects which state properties are merged into the component's props
        dispatch => bindActionCreators({
            ...roomActionCreators,
            ...groupActionCreators,
            ...sessionActionCreators,
            ...unitActionCreators
        }, dispatch) // Selects which action creators are merged into the component's props
    )(GroupDetail as any)));
