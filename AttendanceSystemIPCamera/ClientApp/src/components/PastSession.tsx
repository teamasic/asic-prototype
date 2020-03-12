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
import SessionViewModel from '../models/SessionViewModel';
import '../styles/Table.css';
import { renderStripedTable } from '../utils'

interface Props {
    group: Group
    redirect: (url: string) => void
}

interface PastSessionComponentState {
    sessions: SessionViewModel[],
    page: number
}

type SessionProps = SessionState &
    Props & // ... state we've requested from the Redux store
    typeof sessionActionCreators & // ... plus action creators we've requested
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class PastSession extends React.PureComponent<SessionProps, PastSessionComponentState> {
    state = {
        sessions: new Array(0),
        page: 1
    }

    public componentDidMount() {
        this.loadSession();
    }

    private reloadSession = (data: Session[]) => {
        var sessions = new Array(0);
        
        data.forEach((value: Session) => {
            var sessionViewModel = {
                id: value.id,
                name: value.name,
                startTime: this.formatDate(value.startTime),
                endTime: this.formatDate(value.endTime)
            }
            sessions.push(sessionViewModel);
        });

        this.setState({
            sessions: sessions
        })
    }

    private appendLeadingZeroes = (n: number) => {
        if (n <= 9) {
            return "0" + n;
        }
        return n
    }

    private onPageChange = (page: number) => {
        this.setState({
            page: page
        })
    }

    private formatDate = (date: Date) => {
        var weekday = new Array(7);
        weekday[0] = "Sun";
        weekday[1] = "Mon";
        weekday[2] = "Tue";
        weekday[3] = "Wed";
        weekday[4] = "Thu";
        weekday[5] = "Fri";
        weekday[6] = "Sat";
        var startTime = new Date(date);
        return weekday[startTime.getDay()] + " " + this.appendLeadingZeroes(startTime.getDate()) + "-" +
            this.appendLeadingZeroes((startTime.getMonth() + 1)) + "-" + startTime.getFullYear() + " " +
            this.appendLeadingZeroes(startTime.getHours()) + ":" + this.appendLeadingZeroes(startTime.getMinutes());
    }

    public render() {
        const columns = [
            {
                title: "#",
                key: "index",
                width: '5%',
                render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
            },
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
                width: '10%',
                render: (text: any, record: any) =>
                    <Button type="primary" onClick={() => this.props.redirect(`session/${record.id}`)} >
                        Detail
                    </Button>
                ,
            }
        ];
        return (
            <Table dataSource={this.state.sessions}
                columns={columns}
                rowKey="id"
                bordered
                pagination={{
                    pageSize: 5,
                    total: this.state.sessions != undefined ? this.state.sessions.length : 0,
                    showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} sessions`,
                    onChange: this.onPageChange
                }}
                rowClassName={renderStripedTable}
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