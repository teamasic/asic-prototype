import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Attendee from '../models/Attendee';
import { ApplicationState } from '../store';
import { groupActionCreators } from '../store/group/actionCreators';
import { GroupsState } from '../store/group/state';
import { Breadcrumb, Icon, Button, Empty } from 'antd';
import { Typography } from 'antd';
import { Tabs } from 'antd'
import GroupInfo from './GroupInfo'
import PastSession from './PastSession'
import classNames from 'classnames';

const { Title } = Typography;
const { Paragraph } = Typography
const { TabPane } = Tabs;

// At runtime, Redux will merge together...
type GroupDetailProps =
    GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters


class GroupDetail extends React.PureComponent<GroupDetailProps> {
    // This method is called when the component is first added to the document
    public componentDidMount() {
        this.ensureDataFetched();
    }

    public onChange = (str: string) => {
        var updateGroupName = {
            ...this.props.selectedGroup,
            name: str
        }
    }

    public render() {
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
                        <Paragraph editable={{ onChange: this.onChange }}>{this.props.selectedGroup.name}</Paragraph>
                    </Title>
                </div>
                <Tabs defaultActiveKey="1" type="card">
                    <TabPane tab="Group Information" key="1">
                        <GroupInfo attendees={this.props.selectedGroup.attendees} />
                    </TabPane>
                    <TabPane tab="Past Session" key="2">
                        <PastSession/>
                    </TabPane>
                </Tabs>
            </React.Fragment>
        );
    }

    private ensureDataFetched() {
        
    }

    private renderEmpty() {
        return <Empty>
        </Empty>;
    }
}

//export default GroupDetail;

export default connect(
    (state: ApplicationState) => state.groups, // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(GroupDetail as any);
