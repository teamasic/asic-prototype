import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { sessionActionCreators } from '../store/session/actionCreators';
import {
    Button,
    Empty,
    Tooltip,
    Modal,
    Input} from 'antd';
import { Typography } from 'antd';
import '../styles/Session.css';
import { SessionState } from '../store/session/state';
import '../styles/ActiveView.css';
import MarkUnknownPresentModal from './MarkUnknownPresentModal';
const { Title } = Typography;
const { confirm } = Modal;

interface State {
    unknownModalVisible: boolean;
    currentUnknownImage: string;
}

interface Props {
    sessionId: number;
    editable: boolean;
    markAsPresent: (attendeeCode: string) => void;
}

type UnknownProps =
    SessionState &
    typeof sessionActionCreators &
    Props &
    RouteComponentProps<{
        id?: string;
    }>; // ... plus incoming routing parameters

class UnknownSection extends React.PureComponent<UnknownProps, State>{

    constructor(props: UnknownProps) {
        super(props);
        this.state = {
            unknownModalVisible: false,
            currentUnknownImage: ''
        };
    }

    private openUnknownModal(currentUnknownImage: string) {
        this.setState({
            unknownModalVisible: true,
            currentUnknownImage
        });
    }
    private closeUnknownModal() {
        this.setState({
            unknownModalVisible: false,
            currentUnknownImage: ''
        });
    }

    private getImageBox(image: string) {
        return <div className="image-wrapper">
            <div className="placeholder"
                style={{
                    backgroundImage:
                        `url(/api/avatars/placeholder.jpg)`
                }}>
                <div className="image"
                    style={{
                        backgroundImage:
                            image
                    }}>
                </div>
            </div>
        </div>;
    }

    private removeImage(img: string) {
        confirm({
            title: "Do you want to remove this unknown attendee?",
            okType: "danger",
            onOk: () => {
                this.props.removeUnknownImage(this.props.sessionId, img);
            }
        });
    }

    public render() {
        const unknowns = this.props.unknownImages.slice().reverse();
        return <div className="container">
            <Title className="title" level={4}>Unknown attendees</Title>
            {
                unknowns.length > 0 ?
                    <div className="box-container fixed-grid--around">
                        {
                            unknowns.map((img, i) =>
                                <div key={i}
                                    className="attendee-box grid-element">
                                    <div className="inner-box">
                                        {this.getImageBox(`url(/api/unknown/${this.props.sessionId}/${img})`)}
                                        {
                                            this.props.editable ? 
                                                <div className="inner-box-actions">
                                                    <Tooltip title="Remove this image">
                                                        <Button
                                                            onClick={() => this.removeImage(img)}
                                                            size="small"
                                                            type="danger"
                                                            shape="circle"
                                                            icon="close" />
                                                    </Tooltip>
                                                    <Tooltip title="Mark attendee present">
                                                        <Button
                                                            onClick={() => this.openUnknownModal(img)}
                                                            className=""
                                                            type="primary"
                                                            size="small"
                                                            shape="circle"
                                                            icon="issues-close" />
                                                    </Tooltip>
                                                </div>
                                                : <></>
                                        }
                                    </div>
                                </div>
                            )
                        }
                    </div> :
                    <Empty description="Unknown attendees will show up here." />
            }
            {
                <MarkUnknownPresentModal
                    unknownImage={this.state.currentUnknownImage}
                    removeUnknownImage={(img: string) => this.props.removeUnknownImage(this.props.sessionId, img)}
                    visible={this.state.unknownModalVisible}
                    hideModal={() => this.closeUnknownModal()}
                    markAsPresent={this.props.markAsPresent}
                    attendeeRecords={this.props.attendeeRecords}
                />
            }
        </div>;
    }

}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.sessions,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(sessionActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(UnknownSection as any);