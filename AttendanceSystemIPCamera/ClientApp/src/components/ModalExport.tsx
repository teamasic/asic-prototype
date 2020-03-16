﻿import * as React from 'react';
import { Modal, DatePicker, Typography, Select, Row, Col, Input, InputNumber, Button, Table } from 'antd'
import { GroupsState } from '../store/group/state';
import { sessionActionCreators } from '../store/session/actionCreators';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import Group from '../models/Group';
import { CSVLink } from 'react-csv'
import moment from 'moment';
import ExportRequest from '../models/ExportRequest';
import { formatDateDDMMYYYYHHmm } from '../utils'
import { renderStripedTable } from '../utils'

const { Text } = Typography
const { Option } = Select
const InputGroup = Input.Group

enum DatePickerOption {
    SINGLE,
    START_DATE,
    END_DATE
}

const TIME_OPTIONS = {
    SINGLE_DATE: 'single_date',
    RANGE_DATE: 'range_date'
}
const ATTENDANCE_OPTIONS = {
    ALL: 'all',
    GREATER_THAN_OR_EQUAL: 'greater',
    LESS_THAN_OR_EQUAL: 'less'
}
const ATTENDANCE_STATUS_OPTIONS = {
    ALL: 'all',
    PRESENT: 'present',
    ABSENT: 'absent'
}

interface Props {
    modalVisible: boolean,
    group: Group,
    closeModal: Function
}

interface ModalExportComponentStates {
    startDate: moment.Moment,
    endDate: moment.Moment,
    singleDate: moment.Moment,
    timePicker: string,
    fileName: string,
    isPresent: string,
    isGreaterThanOrEqual: string,
    attendancePercent: number,
    csvData: any,
    exportStatus: string,
    isGenerated: boolean,
    isGenerating: boolean,
    tblReviewColumns: any,
    page: number
}

// At runtime, Redux will merge together...
type ModalExportProps =
    GroupsState// ... state we've requested from the Redux store
    & Props
    & typeof sessionActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class ModalExport extends React.PureComponent<ModalExportProps, ModalExportComponentStates> {
    state = {
        startDate: moment().subtract(1, "days"),
        endDate: moment(),
        singleDate: moment(),
        timePicker: TIME_OPTIONS.RANGE_DATE,
        fileName: "",
        isPresent: ATTENDANCE_STATUS_OPTIONS.ALL,
        isGreaterThanOrEqual: ATTENDANCE_OPTIONS.ALL,
        attendancePercent: 100,
        csvData: [],
        exportStatus: "Generate",
        isGenerated: false,
        isGenerating: false,
        tblReviewColumns: new Array(0),
        page: 1
    }

    private generate = () => {
        if (!this.state.isGenerated) {
            this.setState({
                isGenerating: true
            })
            this.setFileName();
            var exportRequest = {
                groupId: this.props.group.id,
                isSingleDate: this.state.timePicker == TIME_OPTIONS.SINGLE_DATE,
                //(Single date && condition is not all) || (Range date && condition is not all)
                withCondition: (this.state.timePicker == TIME_OPTIONS.SINGLE_DATE && this.state.isPresent != ATTENDANCE_STATUS_OPTIONS.ALL) ||
                    (this.state.timePicker != TIME_OPTIONS.SINGLE_DATE && this.state.isGreaterThanOrEqual != ATTENDANCE_OPTIONS.ALL),
                singleDate: this.state.singleDate.format("YYYY-MM-DD"),
                startDate: this.state.startDate.format("YYYY-MM-DD"),
                endDate: this.state.endDate.format("YYYY-MM-DD"),
                isPresent: this.state.isPresent == ATTENDANCE_STATUS_OPTIONS.PRESENT,
                isGreaterThanOrEqual: this.state.isGreaterThanOrEqual == ATTENDANCE_OPTIONS.GREATER_THAN_OR_EQUAL,
                attendancePercent: this.state.attendancePercent
            };
            this.props.startGenerateExport(exportRequest, this.generateSuccess, this.setCsvData);
            
        }
        //this.handleCancel();
    }

    private generateSuccess = (exportRequest: ExportRequest) => {
        setTimeout(() => {
            this.setState({
                isGenerated: true,
                isGenerating: false,
                exportStatus: "Export"
            });
        }, 1000);
        this.generateColumns(exportRequest);
    }

    private handleCancel = () => {
        this.props.closeModal();
        this.setState({
            isGenerated: false,
            isGenerating: false,
            exportStatus: "Generate"
        });
    }

    private onDatePickerChange = (date: any, dateString: string, datePicker: DatePickerOption) => {
        switch (datePicker) {
            case DatePickerOption.SINGLE:
                this.setState({
                    singleDate: moment(dateString)
                });
                break;
            case DatePickerOption.START_DATE:
                this.setState({
                    startDate: moment(dateString)
                });
                break;
            case DatePickerOption.END_DATE:
                this.setState({
                    endDate: moment(dateString)
                });
                break;
        }
    }

    private onTimePickerChange = (value: string) => {
        this.setState({
            timePicker: value
        });
    }

    private onAttendanceStatusChange = (value: string) => {
        this.setState({
            isPresent: value
        });
    }

    private onAttendanceOptionChange = (value: string) => {
        this.setState({
            isGreaterThanOrEqual: value
        });
    }

    private onPercentChange = (value: number | undefined) => {
        if (value != undefined) {
            this.setState({
                attendancePercent: JSON.parse(value.toString())
            });
        }
    }

    private setCsvData = (data: any) => {
        this.setState({
            csvData: data
        });
    }

    private setFileName = () => {
        var updatedFileName = this.props.group.code + "_" + this.props.group.name;
        if (this.state.timePicker == TIME_OPTIONS.SINGLE_DATE) {
            var date = this.state.singleDate;
            updatedFileName += "_" + date.toISOString().substring(0, 10);
            if (this.state.isPresent != ATTENDANCE_STATUS_OPTIONS.ALL) {
                updatedFileName += "_" + this.state.isPresent + ".csv";
            } else {
                updatedFileName += ".csv";
            }
            this.setState({
                fileName: updatedFileName
            });
            return;
        } else {
            var date = this.state.startDate;
            updatedFileName += "_" + date.toISOString().substring(0, 10);
            date = this.state.endDate;
            updatedFileName += "_" + date.toISOString().substring(0, 10);
            if (this.state.isGreaterThanOrEqual != ATTENDANCE_OPTIONS.ALL) {
                updatedFileName += "_" + this.state.isGreaterThanOrEqual + "_" + this.state.attendancePercent + "percent" + ".csv";
            } else {
                updatedFileName += ".csv";
            }
            this.setState({
                fileName: updatedFileName
            });
        }
    }

    private parseInputNumber = (value: string | undefined) => {
        if (value != undefined)
            return JSON.parse(value.replace('%', ''));
    }

    private generateColumns = (exportRequest: ExportRequest) => {
        var columns = new Array(0);
        if (exportRequest.isSingleDate || (!exportRequest.isSingleDate && !exportRequest.withCondition)) {
            columns = [
                {
                    title: "#",
                    key: "index",
                    width: '5%',
                    render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
                },
                {
                    title: 'Session',
                    key: 'sessionName',
                    dataIndex: 'sessionName'
                },
                {
                    title: 'Start Time',
                    key: 'startTime',
                    dataIndex: 'startTime',
                    render: (text: any, record: any, index: number) => formatDateDDMMYYYYHHmm(text)
                    
                },
                {
                    title: 'End Time',
                    key: 'endTime',
                    dataIndex: 'endTime',
                    render: (text: any, record: any, index: number) => formatDateDDMMYYYYHHmm(text)
                },
                {
                    title: 'Code',
                    key: 'attendeeCode',
                    dataIndex: 'attendeeCode'
                },
                {
                    title: 'Name',
                    key: 'attendeeName',
                    dataIndex: 'attendeeName'
                },
                {
                    title: 'Present',
                    key: 'present',
                    dataIndex: 'present'
                }
            ]
        } else {
            columns = [
                {
                    title: "#",
                    key: "index",
                    width: '5%',
                    render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
                },
                {
                    title: 'Code',
                    key: 'attendeeCode',
                    dataIndex: 'attendeeCode'
                },
                {
                    title: 'Name',
                    key: 'attendeeName',
                    dataIndex: 'attendeeName'
                },
                {
                    title: 'Attendance percent',
                    key: 'attendancePercent',
                    dataIndex: 'attendancePercent'
                }
            ]
        }
        this.setState({
            tblReviewColumns: columns
        })
    }

    private onPageChange = (page: number) => {
        this.setState({
            page: page
        })
    }

    private onExport = () => {
        setTimeout(() => {
            Modal.success({
                content: 'Export successfully'
            });
        }, 1000);
        //Close Review modal
        this.setState({
            isGenerated: false
        });
        //Close Export modal
        this.handleCancel();
    }

    public render() {
        var modalTitle = this.props.group.code + " - " + this.props.group.name;
        return (
            <div>
                <Modal
                    visible={this.props.modalVisible}
                    title={modalTitle}
                    centered
                    okText={this.state.isGenerating ?
                        ("Generating") : ("Generate")
                    }
                    okButtonProps={{ loading: this.state.isGenerating }}
                    onOk={this.generate}
                    onCancel={this.handleCancel}
                >
                    <Row style={{ marginBottom: 10 }}>
                        <Col span={5}>
                            <Text strong>Time: </Text>
                        </Col>
                        <Col span={19}>
                            <Select defaultValue={TIME_OPTIONS.RANGE_DATE}
                                style={{ width: '100%' }}
                                onChange={this.onTimePickerChange}>
                                <Option value={TIME_OPTIONS.SINGLE_DATE}>In a date</Option>
                                <Option value={TIME_OPTIONS.RANGE_DATE}>In a range of date</Option>
                            </Select>
                        </Col>
                    </Row>
                    <Row style={{ marginBottom: 10 }}>
                        <Col span={5}>
                            <Text strong>Choose Date: </Text>
                        </Col>
                        <Col span={19}>
                            {
                                this.state.timePicker == TIME_OPTIONS.SINGLE_DATE ?
                                    (
                                        <DatePicker onChange={(date: any, dateString: string) => this.onDatePickerChange(date, dateString, DatePickerOption.SINGLE)}
                                            value={this.state.singleDate}
                                            style={{ width: '50%' }} />
                                    )
                                    :
                                    (
                                        <InputGroup compact>
                                            <DatePicker onChange={(date: any, dateString: string) => this.onDatePickerChange(date, dateString, DatePickerOption.START_DATE)}
                                                value={this.state.startDate}
                                                style={{ width: '50%' }} />
                                            <DatePicker onChange={(date: any, dateString: string) => this.onDatePickerChange(date, dateString, DatePickerOption.END_DATE)}
                                                value={this.state.endDate}
                                                style={{ width: '50%' }} />
                                        </InputGroup>
                                    )
                            }
                        </Col>
                    </Row>
                    <Row>
                        <Col span={5}>
                            <Text strong>Condition: </Text>
                        </Col>
                        <Col span={19}>
                            {this.state.timePicker == TIME_OPTIONS.SINGLE_DATE ?
                                (
                                    <Select defaultValue={ATTENDANCE_STATUS_OPTIONS.ALL} style={{ width: '50%' }}
                                        onChange={this.onAttendanceStatusChange}>
                                        <Option value={ATTENDANCE_STATUS_OPTIONS.ALL}>All</Option>
                                        <Option value={ATTENDANCE_STATUS_OPTIONS.PRESENT}>Is Present</Option>
                                        <Option value={ATTENDANCE_STATUS_OPTIONS.ABSENT}>Is Absent</Option>
                                    </Select>
                                ) :
                                (
                                    <InputGroup compact>
                                        <Select defaultValue={ATTENDANCE_OPTIONS.ALL} style={{ width: '70%' }}
                                            onChange={this.onAttendanceOptionChange}>
                                            <Option value={ATTENDANCE_OPTIONS.ALL}>
                                                All
                                         </Option>
                                            <Option value={ATTENDANCE_OPTIONS.GREATER_THAN_OR_EQUAL}>
                                                Presence Greater Than Or Equal To
                                         </Option>
                                            <Option value={ATTENDANCE_OPTIONS.LESS_THAN_OR_EQUAL}>
                                                Presence Less Than Or Equal To
                                         </Option>
                                        </Select>
                                        {this.state.isGreaterThanOrEqual != ATTENDANCE_OPTIONS.ALL ?
                                            (<InputNumber
                                                defaultValue={this.state.attendancePercent}
                                                min={0}
                                                max={100}
                                                formatter={value => `${value}%`}
                                                parser={this.parseInputNumber}
                                                onChange={this.onPercentChange}
                                                style={{ width: '30%' }}
                                            />) : null
                                        }
                                    </InputGroup>
                                )
                            }
                        </Col>
                    </Row >
                </Modal>

                <Modal
                    title="Review"
                    visible={this.state.isGenerated}
                    centered
                    width='80%'
                    okText={
                        <CSVLink data={this.state.csvData}
                            filename={this.state.fileName}
                            onClick={this.onExport}
                        >
                            Export
                        </CSVLink>}
                    onCancel={() => this.setState({ isGenerated: false })}
                >
                    <Table
                        dataSource={this.state.csvData}
                        columns={this.state.tblReviewColumns}
                        rowKey='$id'
                        bordered
                        pagination={{
                            pageSize: 5,
                            total: this.state.csvData != undefined ? this.state.csvData.length : 0,
                            showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} rows`,
                            onChange: this.onPageChange
                        }}
                        rowClassName={renderStripedTable}
                    />
                </Modal>
            </div>
        );
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) =>
        ({
            ...state,
            ...ownProps
        }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(ModalExport as any);