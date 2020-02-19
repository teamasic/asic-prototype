import * as React from 'react';
import { Table } from 'antd'
import { GroupsState } from '../store/group/state';
import Group from '../models/Group';
import { groupActionCreators } from '../store/group/actionCreators';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import Attendee from '../models/Attendee';

interface Props {
    attendees?: Attendee[]
}

// At runtime, Redux will merge together...
type GroupInfoProps =
    Props 
    & GroupsState // ... state we've requested from the Redux store
    & typeof groupActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class GroupInfo extends React.PureComponent<GroupInfoProps> {
    public render() {
        console.log(this.props.attendees);
        const columns = [
            {
                title: 'Code',
                key: 'code',
                dataIndex: 'code'
            },
            {
                title: 'Name',
                key: 'name',
                dataIndex: 'name'
            }
        ];

        return (
            <Table dataSource={this.props.attendees} columns={columns} rowKey="$id"
                pagination={{ pageSize: 5 }}
            />
            );
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.groups,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(groupActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(GroupInfo as any);