import * as React from 'react';
import { Modal, DatePicker, message, Typography } from 'antd'
import { GroupsState } from '../store/group/state';
import { sessionActionCreators } from '../store/session/actionCreators';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import Group from '../models/Group';

const { RangePicker } = DatePicker
const { Text } = Typography

interface Props {
    modalVisible: boolean,
    group: Group,
    closeModal: Function
}

interface ModalExportComponentStates {
    startDate: Date,
    endDate: Date
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
        endDate: new Date()
    }

    public export = () => {
        this.props.startExportSession
            (this.props.group.id,
                this.state.startDate,
                this.state.endDate,
                this.exportSuccess);
    }

    public exportSuccess = () => {
        this.handleCancel();
        message.success("Export attendance data success!");
    }

    public handleCancel = () => {
        this.props.closeModal();
    }

    public onCalendarChange = (dates: any, dateString: [string, string]) => {
        console.log(dateString[0]);
        console.log(dateString[1]);
        this.setState({
            startDate: new Date(dateString[0]),
            endDate: new Date(dateString[1])
        });
    }

    public render() {
        var modalTitle = this.props.group.id + " - " + this.props.group.name;
        return (
            <Modal
                visible={this.props.modalVisible}
                title={modalTitle}
                centered
                okText="Export"
                onOk={this.export}
                onCancel={this.handleCancel}
            >
                <Text>Choose Date: </Text><RangePicker onChange={this.onCalendarChange} />
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