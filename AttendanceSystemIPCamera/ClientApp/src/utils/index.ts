import format from 'date-fns/format';
import moment from 'moment';

export const formatFullDateTimeString = (time: Date | string) => format(new Date(time), 'EEEE, MMMM d, yyyy');

export const formatDateDDMMYYYYHHmm = (time: Date | string) => format(new Date(time), 'dd-MM-yyyy hh:mm');

export const minutesOfDay = (time: moment.Moment) => {
    return time.minutes() + time.hours() * 60;
}

export const renderStripedTable = (record: any, index: number) => {
    if (index % 2 == 0) {
        return 'default';
    } else {
        return 'striped';
    }
}