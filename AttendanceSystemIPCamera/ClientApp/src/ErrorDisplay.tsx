import * as React from 'react';
import { Result, Button } from 'antd';
import { RouteComponentProps } from 'react-router';
import { Link, withRouter } from 'react-router-dom';

const ErrorDisplay = (props: RouteComponentProps) => {
    return <Result
        status="500"
        title="There was a problem serving the requested page."
        subTitle="Usually this means an unexpected error happened while processing your request."
        extra={
            <Button type="primary" onClick={() => {
                props.history.goBack();
            }}>Go back and try again</Button>
        }
    />;
};

export default withRouter(ErrorDisplay);