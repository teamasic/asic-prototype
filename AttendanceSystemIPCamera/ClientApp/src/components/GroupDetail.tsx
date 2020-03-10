import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Breadcrumb, Icon, Button, Empty, message } from 'antd';
import { Typography } from 'antd';
import { Tabs } from 'antd'
import GroupInfo from './GroupInfo'
import PastSession from './PastSession'
import ModalExport from './ModalExport'
import classNames from 'classnames';

const { Title } = Typography;
const { Paragraph } = Typography
const { TabPane } = Tabs;

// At runtime, Redux will merge together...
type GroupDetailProps =
    GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{
        id?: string;
    }>; // ... plus incoming routing parameters


class GroupDetail extends React.PureComponent<GroupDetailProps> {
    state = {
        modalExportVisible: false
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

    public closeModalExport = () => {
        this.setState({
            modalExportVisible: false
        })
    }

    private redirect(url: string) {
        this.props.history.push(url);
    }

    public render() {
        const exportModal = <Button type="default" onClick={this.openModalExport} icon="export">Export</Button>
        return (
            <React.Fragment>
                <div className="breadcrumb-container">
                    <Breadcrumb>
                        <Breadcrumb.Item href="">
                            <Icon type="home" />
                        </Breadcrumb.Item>
                        <Breadcrumb.Item>
                            <Icon type="hdd" />
                            <span>Group</span>
                        </Breadcrumb.Item>
                        <Breadcrumb.Item>
                            <span>{this.props.selectedGroup.code} - {this.props.selectedGroup.name}</span>
                        </Breadcrumb.Item>
                    </Breadcrumb>
                </div>
                <div className="title-container">
                    <Title className="title" level={3}>
                        <Paragraph editable={{ onChange: this.editGroupName }}>{this.props.selectedGroup.name}</Paragraph>
                    </Title>
                </div>
                <Tabs defaultActiveKey="1" type="card" tabBarExtraContent={exportModal}>
                    <TabPane tab="Group Information" key="1">
                        <GroupInfo attendees={this.props.selectedGroup.attendees} />
                    </TabPane>
                    <TabPane tab="Past Session" key="2">
                        <PastSession group={this.props.selectedGroup} redirect={url => this.redirect(url)} />
                    </TabPane>
                </Tabs>
                <ModalExport modalVisible={this.state.modalExportVisible}
                    group={this.props.selectedGroup}
                    closeModal={this.closeModalExport} />
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        var strId = this.props.match.params.id;
        if (strId) {
            var intId = parseInt(strId);
            this.props.requestGroupDetail(intId);
        }
    }

    private renderEmpty() {
        return <Empty>
        </Empty>;
    }
}

export default connect(
    (state: ApplicationState) => state.groups, // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(GroupDetail as any);
