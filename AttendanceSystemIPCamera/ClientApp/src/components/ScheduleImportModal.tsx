import * as React from 'react';
import { Modal, Upload, Row, Col, Button, Icon, Typography, Table } from 'antd';
import { renderStripedTable } from '../utils';
import { parse } from 'papaparse';
import { isNullOrUndefined } from 'util';
import { RouteComponentProps, withRouter } from 'react-router';
import { ApplicationState } from '../store';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import ScheduleCreate from '../models/ScheduleCreate';
import { GroupsState } from '../store/group/state';
import { sessionActionCreators } from '../store/session/actionCreators';
import { SessionState } from '../store/session/state';

const { Text } = Typography;

interface Props {
    modalVisible: boolean,
    handleCancel: Function,
    reloadSchedules: Function
}

interface ScheduleImportModalComponentState {
    page: number,
    importSchedules: any,
    msgImportCSV: string,
    csvFile: File,
    buttonSaveLoading: boolean,
    buttonSaveDisable: boolean
}

type ScheduleImportModalProps =
    SessionState
    & GroupsState
    & Props
    & typeof sessionActionCreators
    & RouteComponentProps<{}>;

class ScheduleImportModal extends React.PureComponent<ScheduleImportModalProps, ScheduleImportModalComponentState> {
    constructor(props: ScheduleImportModalProps) {
        super(props);
        this.state = {
            page: 1,
            importSchedules: [],
            msgImportCSV: ' ',
            csvFile: new File([], 'Null'),
            buttonSaveLoading: false,
            buttonSaveDisable: false
        }
    }

    private handleSubmit = (e: any) => {
        e.preventDefault();
        var isImportedFile = this.state.csvFile.name !== 'Null';
        if (!isImportedFile) {
            this.setState({ msgImportCSV: 'Please import csv file' });
        } else {
            this.setState({ buttonSaveLoading: true });
            var schedules = new Array<ScheduleCreate>(0);
            var importSchedules = this.state.importSchedules;
            importSchedules.forEach((item: any) => {
                var schedule = {
                    groupCode: this.props.selectedGroup.code,
                    slot: item.slot,
                    room: item.room,
                    date: item.date
                };
                schedules.push(schedule);
            });
            console.log(schedules);
            this.props.requestCreateScheduledSession(schedules, () => {
                this.setState({ buttonSaveLoading: false });
                this.props.reloadSchedules();
                this.handleCancel();
            });
        }
    }

    private handleCancel = () => {
        this.props.handleCancel();
        this.resetState();
    }

    private resetState = () => {
        this.setState({
            page: 1,
            importSchedules: [],
            msgImportCSV: ' ',
            csvFile: new File([], 'Null')
        });
    }

    private onPageChange = (page: number) => {
        this.setState({ page: page });
    }

    private validateBeforeUpload = (file: File): boolean | Promise<void> => {
        if (file.type !== "application/vnd.ms-excel") {
            this.setState({ msgImportCSV: 'Only accept CSV file!' });
            return false;
        }
        return this.parseFileToTable(file);
    }

    private parseFileToTable = (file: File): Promise<void> => {
        return new Promise((resolve, reject) => {
            var thisState = this;
            parse(file, {
                header: true,
                complete: function (results: any, file: File) {
                    if (thisState.checkValidFileFormat(results.data)) {
                        thisState.setState({
                            importSchedules: results.data,
                            msgImportCSV: '',
                            csvFile: file,
                            buttonSaveDisable: false
                        }, () => {
                            resolve();
                        });
                        var totalSession = thisState.props.selectedGroup.totalSession;
                        console.log(totalSession + "|" + thisState.state.importSchedules.length);
                        if (thisState.state.importSchedules.length > totalSession) {
                            thisState.setState({
                                buttonSaveDisable: true,
                                msgImportCSV: 'You cannot import more than '
                                    + totalSession
                                    + ' schedules for this group'
                            });
                        }
                    } else {
                        thisState.setState({
                            importSchedules: [],
                            msgImportCSV: 'You upload a csv file with wrong format. Please try again!'
                        }, () => { reject(); });
                    }
                }, error: function (errors: any, file: File) {
                    thisState.setState({
                        importSchedules: [],
                        msgImportCSV: "Upload error: " + errors
                    }, () => { reject(); });
                }
            });
        });
    }

    public checkValidFileFormat = (schedules: []) => {
        let temp: { slot: string, room: string, date: string }[] = schedules;
        if (temp.length > 0) {
            if (!isNullOrUndefined(temp[0].slot)
                && !isNullOrUndefined(temp[0].room)
                && !isNullOrUndefined(temp[0].date)) {
                return true;
            }
        }
        return false;
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
                title: 'Slot',
                key: 'slot',
                dataIndex: 'slot'
            },
            {
                title: 'Room',
                key: 'room',
                dataIndex: 'room'
            },
            {
                title: 'Date',
                key: 'date',
                dataIndex: 'date'
            }
        ];
        var isLoading = this.state.buttonSaveLoading;
        var disable = this.state.buttonSaveDisable;
        return (
            <React.Fragment>
                <Modal
                    visible={this.props.modalVisible}
                    title="Import schedule"
                    okText="Save"
                    centered
                    width='80%'
                    onOk={this.handleSubmit}
                    okButtonProps={{ loading: isLoading, disabled: disable }}
                    onCancel={this.handleCancel}
                >
                    <Row gutter={[0, 32]}>
                        <Col span={4}>
                            <Upload
                                multiple={false}
                                accept=".csv"
                                showUploadList={false}
                                beforeUpload={this.validateBeforeUpload}
                            >
                                <Button>
                                    <Icon type="upload" /> Upload CSV File
                                    </Button>
                            </Upload>
                        </Col>
                        <Col span={20}>
                            {this.state.msgImportCSV.length === 0 ?
                                (
                                    <span>
                                        <Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />
                                        <Text type="secondary"> {this.state.csvFile.name}</Text>
                                    </span>
                                ) :
                                this.state.msgImportCSV.length !== 1 ?
                                    (<Icon type="close-circle" theme="twoTone" twoToneColor="#ff0000" />) : null
                            }
                            <Text type="danger"> {this.state.msgImportCSV}</Text>
                        </Col>
                    </Row>
                    <Row>
                        <Table dataSource={this.state.importSchedules}
                            columns={columns}
                            bordered
                            rowClassName={renderStripedTable}
                            pagination={{
                                pageSize: 5,
                                total: this.state.importSchedules != undefined ? this.state.importSchedules.length : 0,
                                showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} rows`,
                                onChange: this.onPageChange
                            }}
                        />;
                        </Row>
                </Modal>
            </React.Fragment>
        );
    }
}

const mapStateToProps = (state: ApplicationState, ownProps: Props) => (
    {
        ...state.sessions,
        ...state.groups,
        ...ownProps
    })

export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(ScheduleImportModal as any)