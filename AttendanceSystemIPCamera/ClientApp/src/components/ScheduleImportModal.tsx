import * as React from 'react';
import { Modal, Upload, Row, Col, Button, Icon, Typography, Table } from 'antd';
import { renderStripedTable } from '../utils';
import { parse } from 'papaparse';
import { isNullOrUndefined } from 'util';

const { Text } = Typography;

interface Props {
    modalVisible: boolean,
    handleCancel: Function
}

interface ScheduleImportModalComponentState {
    page: number,
    importSchedules: any,
    msgImportCSV: string,
    csvFile: File
}

class ScheduleImportModal extends React.PureComponent<Props, ScheduleImportModalComponentState> {
    constructor(props: Props) {
        super(props);
        this.state = {
            page: 1,
            importSchedules: [],
            msgImportCSV: ' ',
            csvFile: new File([], 'Null')
        }
    }

    private handleSubmit = () => {

    }

    private handleCancel = () => {
        this.props.handleCancel();
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
                    console.log(results);
                    if (thisState.checkValidFileFormat(results.data)) {
                        thisState.setState({
                            importSchedules: results.data,
                            msgImportCSV: '',
                            csvFile: file
                        }, () => {
                            resolve();
                        });
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
        return (
            <React.Fragment>
                <Modal
                    visible={this.props.modalVisible}
                    title="Import schedule"
                    okText="Save"
                    centered
                    width='80%'
                    onOk={this.handleSubmit}
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

export default ScheduleImportModal;