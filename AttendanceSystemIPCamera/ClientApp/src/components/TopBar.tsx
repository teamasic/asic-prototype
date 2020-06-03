import * as React from 'react';
import { Link, withRouter, RouteComponentProps } from 'react-router-dom';
import { Button, Icon, Breadcrumb } from 'antd';

interface Props {
    showHome?: boolean;
}

// At runtime, Redux will merge together...
type TopBarProps = Props & RouteComponentProps<{}>; // ... plus incoming routing parameters

const TopBar: React.FunctionComponent<TopBarProps> = ({
    children,
    showHome = true,
    history
}) => {
    return (
        <div className="breadcrumb-container">
            <Button type="link"
                size="large"
                onClick={() => history.goBack()}
                icon="arrow-left" />
            <Breadcrumb>
                {
                    showHome && <Breadcrumb.Item onClick={() => history.push("/")}>
                        <Icon type="home" />
                    </Breadcrumb.Item>
                }
                {
                    children
                }
            </Breadcrumb>
        </div>
    );
};

TopBar.defaultProps = {
    showHome: true
} as Partial<TopBarProps>;

export default withRouter(TopBar);