import * as React from 'react';
import { Table, Popconfirm, Button, message, Modal, Form, Input } from 'antd'
import Group from '../models/Group';
import { SessionState } from '../store/session/state';
import { sessionActionCreators } from '../store/session/actionCreators';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import Session from '../models/Session';

interface Props {
    group: Group
    redirect: (url: string) => void
}

interface PastSessionComponentState {
    sessions: Session[]
}

type SessionProps = SessionState &
    Props & // ... state we've requested from the Redux store
    typeof sessionActionCreators & // ... plus action creators we've requested
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class PastSession extends React.PureComponent<SessionProps, PastSessionComponentState> {
    state = {
        sessions: new Array(0)
    }

    public componentDidMount() {
        this.loadSession();
    }

    private reloadSession = (data: Session[]) => {
        this.setState({
            sessions: data
        })
    }

    public render() {
        const columns = [
            {
                title: 'Start Time',
                key: 'startTime',
                dataIndex: 'startTime'
            },
            {
                title: 'End Time',
                key: 'endTime',
                dataIndex: 'endTime'
            },
            {
                title: 'Name',
                key: 'name',
                dataIndex: 'name'
            },
            {
                title: 'Action',
                dataIndex: 'action',
                render: (text: any, record: any) =>
                    <Button type="primary" onClick={() => this.props.redirect(`session/${record.id}`)}>
                        Detail
                    </Button>
                ,
            }
        ];
        return (
            <Table dataSource={this.state.sessions}
                columns={columns}
                rowKey="$id"
                pagination={{ pageSize: 5 }}
            />
        );
    }

    private loadSession = () => {
        this.props.startGetPastSession(this.props.group.id, this.reloadSession);
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.sessions,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(PastSession as any);