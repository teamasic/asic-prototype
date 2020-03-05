import * as React from 'react';
import { Modal, DatePicker, message, Typography, Select, Row, Col, Input, InputNumber } from 'antd'
import { GroupsState } from '../store/group/state';
import { sessionActionCreators } from '../store/session/actionCreators';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import Group from '../models/Group';
import { CSVLink } from 'react-csv'

const { RangePicker } = DatePicker
const { Text } = Typography
const { Option } = Select
const InputGroup = Input.Group
const TIME_OPTIONS = {
    SINGLE_DATE: 'single_date',
    RANGE_DATE: 'range_date'
}
const CONDITION_OPTIONS = {
    WITH_CONDITION: 'with_condition',
    WITHOUT_CONDITION: 'without_condition'
}
const ATTENDANCE_OPTIONS = {
    GREATER_THAN_OR_EQUAL: 'greater',
    LESS_THAN_OR_EQUAL: 'less'
}
const ATTENDANCE_STATUS_OPTIONS = {
    PRESENT: 'present',
    ABSENT: 'absent'
}

interface Props {
    modalVisible: boolean,
    group: Group,
    closeModal: Function
}

interface ModalExportComponentStates {
    startDate: Date,
    endDate: Date,
    singleDate: Date,
    timePicker: string,
    condition: string,
    fileName: string,
    isPresent: string,
    isGreaterThanOrEqual: string,
    attendancePercent: number,
    csvData: any,
    generate: string,
    isGenerated: boolean
}

// At runtime, Redux will merge together...
type ModalExportProps =
    GroupsState// ... state we've requested from the Redux store
    & Props
    & typeof sessionActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class ModalExport extends React.PureComponent<ModalExportProps, ModalExportComponentStates> {
    state = {
        startDate: new Date(),
        endDate: new Date(),
        singleDate: new Date(),
        timePicker: TIME_OPTIONS.RANGE_DATE,
        condition: CONDITION_OPTIONS.WITHOUT_CONDITION,
        fileName: "",
        isPresent: ATTENDANCE_STATUS_OPTIONS.PRESENT,
        isGreaterThanOrEqual: ATTENDANCE_OPTIONS.GREATER_THAN_OR_EQUAL,
        attendancePercent: 100,
        csvData: [],
        generate: "Generate",
        isGenerated: false
    }

    private export = () => {
        if (!this.state.isGenerated) {
            this.setFileName();
            var exportRequest = {
                groupId: this.props.group.id,
                isSingleDate: this.state.timePicker == TIME_OPTIONS.SINGLE_DATE,
                withCondition: this.state.condition == CONDITION_OPTIONS.WITH_CONDITION,
                singleDate: this.state.singleDate,
                startDate: this.state.startDate,
                endDate: this.state.endDate,
                isPresent: this.state.isPresent == ATTENDANCE_STATUS_OPTIONS.PRESENT,
                isGreaterThanOrEqual: this.state.isGreaterThanOrEqual == ATTENDANCE_OPTIONS.GREATER_THAN_OR_EQUAL,
                attendancePercent: this.state.attendancePercent
            };
            console.log(exportRequest);
            this.props.startExportSession
                (exportRequest, this.exportSuccess, this.setCsvData);
            return false;
        }
        this.handleCancel();
        return true;
    }

    private exportSuccess = () => {
        this.setState({
            generate: "Export",
            isGenerated: true
        })
        console.log(this.state.fileName);
    }

    private handleCancel = () => {
        this.props.closeModal();
        this.setState({
            generate: "Generate",
            isGenerated: false
        });
    }

    private onRangePickerChange = (dates: any, dateString: [string, string]) => {
        this.setState({
            startDate: new Date(dateString[0]),
            endDate: new Date(dateString[1])
        });
    }

    private onDatePickerChange = (date: any, dateString: string) => {
        this.setState({
            singleDate: new Date(dateString)
        });
    }

    private onTimePickerChange = (value: string) => {
        this.setState({
            timePicker: value
        });
    }

    private onConditionChange = (value: string) => {
        this.setState({
            condition: value
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
            if (this.state.condition == CONDITION_OPTIONS.WITH_CONDITION) {
                updatedFileName += "_" + this.state.isPresent + ".csv";
            } else {
                updatedFileName += ".cvs";
            }
            this.setState({
                fileName: updatedFileName
            });
            return;
        } else {
            var date = this.state.startDate;
            updatedFileName += "_" +  date.toISOString().substring(0, 10);
            date = this.state.endDate;
            updatedFileName += "_" +  date.toISOString().substring(0, 10);
            if (this.state.condition == CONDITION_OPTIONS.WITH_CONDITION) {
                updatedFileName += "_" +  this.state.condition + "_" + this.state.attendancePercent + ".csv";
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

    public render() {
        var modalTitle = this.props.group.id + " - " + this.props.group.name;
        return (
            <Modal
                visible={this.props.modalVisible}
                title={modalTitle}
                centered
                okText={
                    <CSVLink data={this.state.csvData}
                        filename={this.state.fileName}
                        enclosingCharacter={`'`}
                        onClick={this.export}
                    >
                        {this.state.generate}
                    </CSVLink>
                }
                onCancel={this.handleCancel}
            >
                <Row style={{ marginBottom: 10 }}>
                    <Col span={5}>
                        <Text>Time: </Text>
                    </Col>
                    <Col span={10}>
                        <Select defaultValue={TIME_OPTIONS.RANGE_DATE} style={{ width: '100%' }}
                            onChange={this.onTimePickerChange}>
                            <Option value={TIME_OPTIONS.SINGLE_DATE}>In a date</Option>
                            <Option value={TIME_OPTIONS.RANGE_DATE}>In a range of date</Option>
                        </Select>
                    </Col>
                </Row>
                <Row style={{ marginBottom: 10 }}>
                    <Col span={5}>
                        <Text>Choose Date: </Text>
                    </Col>
                    <Col span={19}>
                        {
                            this.state.timePicker == TIME_OPTIONS.SINGLE_DATE ?
                                (<DatePicker onChange={this.onDatePickerChange} />)
                                : (<RangePicker onChange={this.onRangePickerChange} style={{ width: '100%' }} />)
                        }
                    </Col>
                </Row>
                <Row style={{ marginBottom: 10 }}>
                    <Col span={5}>
                        <Text>Condition: </Text>
                    </Col>
                    <Col span={10}>
                        <Select defaultValue={CONDITION_OPTIONS.WITHOUT_CONDITION} style={{ width: '100%' }}
                            onChange={this.onConditionChange}>
                            <Option value={CONDITION_OPTIONS.WITHOUT_CONDITION}>Without Condition</Option>
                            <Option value={CONDITION_OPTIONS.WITH_CONDITION}>With Condition</Option>
                        </Select>
                    </Col>
                </Row>
                {this.state.condition != CONDITION_OPTIONS.WITHOUT_CONDITION ?
                    (
                        <Row>
                            <Col span={5}>
                                <Text>Attendance </Text>
                            </Col>
                            <Col span={19}>
                                {this.state.timePicker == TIME_OPTIONS.SINGLE_DATE ?
                                    (
                                        <Select defaultValue={ATTENDANCE_STATUS_OPTIONS.PRESENT} style={{ width: '50%' }}
                                            onChange={this.onAttendanceStatusChange}>
                                            <Option value={ATTENDANCE_STATUS_OPTIONS.PRESENT}>Is Present</Option>
                                            <Option value={ATTENDANCE_STATUS_OPTIONS.ABSENT}>Is Absent</Option>
                                        </Select>
                                    ) :
                                    (
                                        <InputGroup compact>
                                            <Select defaultValue={ATTENDANCE_OPTIONS.GREATER_THAN_OR_EQUAL} style={{ width: '70%' }}
                                                onChange={this.onAttendanceOptionChange}>
                                                <Option value={ATTENDANCE_OPTIONS.GREATER_THAN_OR_EQUAL}>
                                                    Greater Than Or Equal To
                                                </Option>
                                                <Option value={ATTENDANCE_OPTIONS.LESS_THAN_OR_EQUAL}>
                                                    Less Than Or Equal To
                                                </Option>
                                            </Select>
                                            <InputNumber
                                                defaultValue={this.state.attendancePercent}
                                                min={0}
                                                max={100}
                                                formatter={value => `${value}%`}
                                                parser={this.parseInputNumber}
                                                onChange={this.onPercentChange}
                                                style={{ width: '30%' }}
                                            />
                                        </InputGroup>
                                    )
                                }
                            </Col>
                        </Row >
                    ) : null
                }
            </Modal>
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