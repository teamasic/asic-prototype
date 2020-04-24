import * as React from 'react';
import { Table, Popconfirm, Button, message, Modal, Form, Input, Typography  } from 'antd'
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
import TableConstants from '../constants/TableConstants';
import SessionStatusConstants from "../constants/SessionStatusConstants";


interface Props {
    group: Group
    redirect: (url: string) => void
}

interface PastSessionComponentState {
    sessions: SessionViewModel[],
    currentPage: number,
    pageSize: number,
    sessionLoading: boolean
}

type SessionProps = SessionState &
    Props & // ... state we've requested from the Redux store
    typeof sessionActionCreators & // ... plus action creators we've requested
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class PastSession extends React.PureComponent<SessionProps, PastSessionComponentState> {
    state = {
        sessions: new Array(0),
        currentPage: 1,
        pageSize: TableConstants.defaultPageSize,
        sessionLoading: false
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
                endTime: this.formatDate(value.endTime),
                status: value.status,
                colorStatus: this.getColorByStatus(value.status)
            }
            sessions.push(sessionViewModel);
        });

        this.setState({
            sessions: sessions,
            sessionLoading: false
        });
    }

    private appendLeadingZeroes = (n: number) => {
        if (n <= 9) {
            return "0" + n;
        }
        return n
    }

    private onPageChange = (page: number) => {
        this.setState({ currentPage: page });
    }

    private onShowSizeChange = (current: number, pageSize: number) => {
        this.setState({
            pageSize: pageSize,
            currentPage: current
        });
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
                render: (text: any, record: any, index: number) => {
                    return (this.state.currentPage - 1) * this.state.pageSize + index + 1
                }
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
                title: 'Status',
                key: 'status',
                dataIndex: 'status',
                render: (text: any, record: any) =>
                    <p  style={{color: record.colorStatus, fontWeight: "bold"}}>{record.status}</p>
            },
            {
                title: 'Action',
                dataIndex: 'action',
                width: '10%',
                render: (text: any, record: any) =>
                    <Button type="primary" onClick={() => this.props.redirect(`/session/${record.id}`)} >
                        Detail
                    </Button>
                ,
            }
        ];
        return (
            <React.Fragment>
                <Table dataSource={this.state.sessions}
                       columns={columns}
                       loading = {this.state.sessionLoading}
                       rowKey="id"
                       bordered
                       pagination={{
                           pageSize: this.state.pageSize,
                           total: this.state.sessions != undefined ? this.state.sessions.length : 0,
                           showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} sessions`,
                           onChange: this.onPageChange,
                           showSizeChanger: true,
                           onShowSizeChange: this.onShowSizeChange
                       }}
                       rowClassName={renderStripedTable}
                />
                <div>
                    <p><span style={{color: this.getColorByStatus(SessionStatusConstants.IN_PROGRESS), fontWeight: "bold"}}>{SessionStatusConstants.IN_PROGRESS}: </span>This session is working, supervisor can take attendance manually or by IP Camera</p>
                    <p><span style={{color: this.getColorByStatus(SessionStatusConstants.EDITABLE), fontWeight: "bold"}}>{SessionStatusConstants.EDITABLE}: </span>This session is not working, but supervisor can still take attendance manually </p>
                    <p><span style={{color: this.getColorByStatus(SessionStatusConstants.FINISHED), fontWeight: "bold"}}>{SessionStatusConstants.FINISHED}: </span>This session is finished, supervisor cannot take attendance</p>
                </div>
            </React.Fragment>

        );
    }

    private loadSession = () => {
        this.setState({ sessionLoading: true });
        this.props.startGetPastSession(this.props.group.code, this.reloadSession);
    }
    private getColorByStatus = (status: string) => {
        if (status == SessionStatusConstants.SCHEDULED){
            return "#3477eb"
        }
        else if (status == SessionStatusConstants.IN_PROGRESS){
            return "#009933"
        }
        else if (status == SessionStatusConstants.EDITABLE){
            return "#ebab34"
        }
        else {
            return "#eb3434"
        }
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.sessions,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(PastSession as any);