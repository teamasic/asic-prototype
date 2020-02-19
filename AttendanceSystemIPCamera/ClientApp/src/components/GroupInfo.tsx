import * as React from 'react';
import { Table } from 'antd'

class GroupInfo extends React.PureComponent {
    public render() {
        const columns = [
            {
                title: 'No.',
                key: 'No',
                dataIndex: 'No'
            },
            {
                title: 'Code',
                key: 'Code',
                dataIndex: 'Code'
            },
            {
                title: 'Name',
                key: 'Name',
                dataIndex: 'Name'
            }
        ];

        const attendees = [
            {
                "No": "1",
                "Code": "SE62823",
                "Name": "Lê Phát Đạt"
            },
            {
                "No": "2",
                "Code": "SE62824",
                "Name": "Lê Hả Hê"
            },
            {
                "No": "3",
                "Code": "SE62825",
                "Name": "Lê Hề Hước"
            },
            {
                "No": "4",
                "Code": "SE62826",
                "Name": "Lê Công Chúa"
            },
            {
                "No": "5",
                "Code": "SE62827",
                "Name": "Lê Tập Gym Cơ Bắp To"
            }
        ]
        return (
            <Table dataSource={attendees} columns={columns} rowKey="No"
                pagination={{ pageSize: 5 }}
            />
            );
    }
}

export default GroupInfo;